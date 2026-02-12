namespace CleanArc.Application.Contracts.Responses.Order
{
    public class CheckoutOrderResponse
    {
        public int OrderId { get; set; }
        public decimal Subtotal { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
