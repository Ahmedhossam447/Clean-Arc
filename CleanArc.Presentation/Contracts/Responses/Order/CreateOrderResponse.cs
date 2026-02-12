namespace CleanArc.Application.Contracts.Responses.Order
{
    public class CreateOrderResponse
    {
        public int OrderId { get; set; }
        public decimal Subtotal { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PaymentUrl { get; set; }
        public List<OrderItemResponse> Items { get; set; }
    }

    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
