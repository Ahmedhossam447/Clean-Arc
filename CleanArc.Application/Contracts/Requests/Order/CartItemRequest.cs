namespace CleanArc.Application.Contracts.Requests.Order
{
    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
