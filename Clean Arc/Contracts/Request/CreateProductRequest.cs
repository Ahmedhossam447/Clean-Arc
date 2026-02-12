namespace Clean_Arc.Contracts.Request
{
    using Microsoft.AspNetCore.Http;

    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile? Image { get; set; }
    }
}
