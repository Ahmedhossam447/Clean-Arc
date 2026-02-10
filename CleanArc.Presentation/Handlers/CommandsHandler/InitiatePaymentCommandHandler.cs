using CleanArc.Application.Commands.Payment;
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
        public InitiatePaymentCommandHandler(IPaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        public async Task<Result<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AmountCents = request.amount * 100;
                string authtoken = await _paymentService.GetAuthTokenAsync();
                int orderid = await _paymentService.CreateOrderAsync(authtoken, AmountCents);
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

                string paymentkey = await _paymentService.GetPaymentKeyAsync(authtoken, orderid, AmountCents, billingData);
                string iframeId = _config["Paymob:IframeId"];
                string url = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentkey}";
                return Result<string>.Success(url);

            }
            catch (Exception ex)
            {
                return Result<string>.Failure(new Error("Payment.Failed", ex.Message));
            }
          
        }
    }
}
