namespace CleanArc.Application.Contracts.Responses.Product
{
    public class CreateProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string ShelterId { get; set; }
    }
}
