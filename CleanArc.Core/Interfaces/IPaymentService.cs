using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GetAuthTokenAsync();
        Task<int> CreateOrderAsync(string token,decimal AmountCents);
        Task<string> GetPaymentKeyAsync(string token, int orderId, decimal amountCents, object billingData);
    }
}
