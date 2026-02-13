namespace CleanArc.Application.Contracts.Responses.Order
{
    public class ShelterSaleResponse
    {
        public int OrderId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public List<ShelterSaleItemResponse> Items { get; set; } = new();
        public decimal TotalSale => Items.Sum(i => i.Price * i.Quantity);
    }

    public class ShelterSaleItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
