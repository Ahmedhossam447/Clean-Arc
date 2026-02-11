using CleanArc.Application.Commands.Payment;
using CleanArc.Application.Common.Security;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Payment
{
    public class ProcessPaymobWebhookCommandHandler : IRequestHandler<ProcessPaymobWebhookCommand, Result<bool>>
    {
        private readonly IPaymobSecurity _paymobSecurity;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessPaymobWebhookCommandHandler(IPaymobSecurity paymobSecurity, IUnitOfWork unitOfWork)
        {
            _paymobSecurity = paymobSecurity;
            _unitOfWork = unitOfWork;
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
                payment.Status = "Successful";
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }

            payment.Status = "Failed";
            await _unitOfWork.SaveChangesAsync();
            return PaymentTransaction.Errors.Failed;
        }
    }
}
