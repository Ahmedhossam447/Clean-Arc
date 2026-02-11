using CleanArc.Application.Commands.Payment;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Handlers.CommandsHandler
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<string>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        public InitiatePaymentCommandHandler(IPaymentService paymentService, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var amountCents = request.amount * 100;
                string authToken = await _paymentService.GetAuthTokenAsync();
                int orderId = await _paymentService.CreateOrderAsync(authToken, amountCents);
                // 4. Billing Data (Paymob requires this)
                // In a real app, you would get this from the User entity or the Request
                var billingData = new
                {
                    first_name = "Ahmed",
                    last_name = "Test",
                    email = "test@test.com",
                    phone_number = "01010101010",
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    state = "Cairo"
                };

                string paymentKey = await _paymentService.GetPaymentKeyAsync(authToken, orderId, amountCents, billingData);
                var transaction = new PaymentTransaction
                {
                    PaymobOrderId = orderId,    
                    UserId = "TestUser123",     
                    UserEmail = "test@test.com",
                    Amount = request.amount,    
                    Currency = "EGP",
                    Status = "Pending",      
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                string iframeId = _config["Paymob:IframeId"];
                string url = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";
                return Result<string>.Success(url);

            }
            catch (Exception ex)
            {
                return Result<string>.Failure(PaymentTransaction.Errors.Failed);
            }
          
        }
    }
}
