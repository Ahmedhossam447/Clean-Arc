using CleanArc.Application.Commands.Payment;
using CleanArc.Application.Common.Security;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Payment
{
    public class ProcessPaymobWebhookCommandHandler : IRequestHandler<ProcessPaymobWebhookCommand, Result<bool>>
    {
        private readonly IPaymobSecurity _paymobSecurity;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymobWebhookCommandHandler> _logger;

        public ProcessPaymobWebhookCommandHandler(
            IPaymobSecurity paymobSecurity,
            IUnitOfWork unitOfWork,
            ILogger<ProcessPaymobWebhookCommandHandler> logger)
        {
            _paymobSecurity = paymobSecurity;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ProcessPaymobWebhookCommand request, CancellationToken cancellationToken)
        {
            if (!_paymobSecurity.ValidateHmac(request.PaymobWebhook.Obj, request.PaymobWebhook.Hmac))
                return PaymentTransaction.Errors.InvalidSignature;

            var payment = (await _unitOfWork.Repository<PaymentTransaction>()
                .GetAsync(t => t.PaymobOrderId == request.PaymobWebhook.Obj.Order.Id))
                .FirstOrDefault();

            if (payment == null)
                return PaymentTransaction.Errors.NotFound;

            if (payment.Status == "Successful")
                return PaymentTransaction.Errors.AlreadyProcessed;

            if (request.PaymobWebhook.Obj.Success)
            {
                // Find the order linked to this payment
                var order = (await _unitOfWork.Repository<Order>()
                    .GetAsync(o => o.PaymentTransactionId == payment.Id))
                    .FirstOrDefault();

                if (order == null)
                {
                    _logger.LogError("Payment {PaymentId} has no linked order", payment.Id);
                    return PaymentTransaction.Errors.NotFound;
                }

                // Decrement stock atomically inside a transaction
                // If ANY item fails, ALL decrements are rolled back
                // Sort by ProductId to prevent deadlocks — all transactions
                // lock rows in the same order, so no circular waits
                var orderItems = (await _unitOfWork.Repository<OrderItem>()
                    .GetAsync(oi => oi.OrderId == order.Id))
                    .OrderBy(oi => oi.ProductId)
                    .ToList();

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach (var item in orderItems)
                    {
                        int rowsAffected = await _unitOfWork.ExecuteSqlRawAsync(
                            "UPDATE Products SET StockQuantity = StockQuantity - {0} WHERE Id = {1} AND StockQuantity >= {0}",
                            item.Quantity, item.ProductId);

                        if (rowsAffected == 0)
                        {
                            _logger.LogWarning(
                                "Insufficient stock for Product {ProductId} (needed {Quantity}) in Order {OrderId}",
                                item.ProductId, item.Quantity, order.Id);

                            // Rollback ALL previous decrements
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                            // Mark order as failed (outside the rolled-back transaction)
                            payment.Status = "Failed";
                            order.Status = "StockUnavailable";
                            await _unitOfWork.SaveChangesAsync();

                            return Order.Errors.InsufficientStock;
                        }
                    }

                    // All items passed — commit decrements + update statuses
                    payment.Status = "Successful";
                    order.Status = "PaymentReceived";
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    _logger.LogError(ex, "Failed to process stock for Order {OrderId}", order.Id);
                    throw;
                }
            }

            // Payment failed
            payment.Status = "Failed";

            var failedOrder = (await _unitOfWork.Repository<Order>()
                .GetAsync(o => o.PaymentTransactionId == payment.Id))
                .FirstOrDefault();

            if (failedOrder != null)
            {
                failedOrder.Status = "PaymentFailed";
            }

            await _unitOfWork.SaveChangesAsync();
            return PaymentTransaction.Errors.Failed;
        }
    }
}
