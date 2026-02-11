using CleanArc.Application.Dtos;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CleanArc.Application.Common.Security
{
    public interface IPaymobSecurity
    {
        bool ValidateHmac(PaymobTransactionObj transaction, string receivedHmac);
    }

    public class PaymobSecurity : IPaymobSecurity
    {
        private readonly string _hmacSecret;

        public PaymobSecurity(IConfiguration configuration)
        {
            _hmacSecret = configuration["Paymob:HmacSecret"]
                ?? throw new InvalidOperationException("Paymob:HmacSecret not configured");
        }

        public bool ValidateHmac(PaymobTransactionObj transaction, string receivedHmac)
        {
            // Paymob requires these fields concatenated in this exact order
            var concatenated = string.Concat(
                transaction.AmountCents,
                transaction.CreatedAt,
                transaction.Currency,
                transaction.ErrorOccured.ToString().ToLower(),
                transaction.HasParentTransaction.ToString().ToLower(),
                transaction.Id,
                transaction.IntegrationId,
                transaction.Is3dSecure.ToString().ToLower(),
                transaction.IsAuth.ToString().ToLower(),
                transaction.IsCapture.ToString().ToLower(),
                transaction.IsRefunded.ToString().ToLower(),
                transaction.IsStandalonePayment.ToString().ToLower(),
                transaction.IsVoided.ToString().ToLower(),
                transaction.Order.Id,
                transaction.Owner,
                transaction.Pending.ToString().ToLower(),
                transaction.SourceData.Pan,
                transaction.SourceData.SubType,
                transaction.SourceData.Type,
                transaction.Success.ToString().ToLower()
            );

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var computedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHmac),
                Encoding.UTF8.GetBytes(receivedHmac.ToLower()));
        }
    }
}
