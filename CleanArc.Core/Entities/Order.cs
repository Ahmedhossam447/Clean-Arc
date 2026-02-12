using CleanArc.Core.Primitives;

namespace CleanArc.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        // Status: Pending -> PaymentReceived -> PaymentFailed
        public string Status { get; set; } = "Pending";

        public decimal Subtotal { get; set; }

        // Link to the Payment Transaction
        public int? PaymentTransactionId { get; set; }
        public PaymentTransaction? PaymentTransaction { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public static class Errors
        {
            public static readonly Error NotFound = new(
                "Order.NotFound",
                "The order with the specified identifier was not found.");

            public static readonly Error Unauthorized = new(
                "Order.Unauthorized",
                "You are not authorized to perform this action on this order.");

            public static readonly Error AlreadyProcessed = new(
                "Order.AlreadyProcessed",
                "This order has already been processed.");

            public static readonly Error EmptyOrder = new(
                "Order.EmptyOrder",
                "Cannot create an order with no items.");

            public static readonly Error InsufficientStock = new(
                "Order.InsufficientStock",
                "One or more products do not have enough stock to fulfill this order.");
        }
    }
}
