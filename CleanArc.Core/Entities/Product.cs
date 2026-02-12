using CleanArc.Core.Primitives;

namespace CleanArc.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }

        public string ShelterId { get; set; }

        public byte[] RowVersion { get; set; }

        public static class Errors
        {
            public static readonly Error NotFound = new(
                "Product.NotFound",
                "The product with the specified identifier was not found.");

            public static readonly Error PhotoUploadFailed = new(
                "Product.PhotoUploadFailed",
                "Failed to upload the product image. Please try again.");

            public static readonly Error Unauthorized = new(
                "Product.Unauthorized",
                "You are not authorized to perform this action on this product.");
        }
    }
}
