namespace CleanArc.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // FK to Order
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // FK to Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Snapshot at time of purchase
        public string ProductName { get; set; }
        public string? PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Shelter who sold it
        public string ShelterId { get; set; }

        // Shipment tracking: Pending → Processing → Shipped → Delivered
        public string Status { get; set; } = "Pending";
    }
}
