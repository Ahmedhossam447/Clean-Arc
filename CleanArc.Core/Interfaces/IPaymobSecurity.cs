using CleanArc.Core.Dtos;

namespace CleanArc.Core.Interfaces
{
    public interface IPaymobSecurity
    {
        bool ValidateHmac(PaymobTransactionObj transaction, string receivedHmac);
    }
}
