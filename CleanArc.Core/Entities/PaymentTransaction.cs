using CleanArc.Core.Primitives;

namespace CleanArc.Core.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public int PaymobOrderId { get; set; }

        public string UserId { get; set; }
        public string? UserEmail { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } = "Pending";
        public string? PaymentReason { get; set; }
        public int? RelatedEntityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static class Errors
        {
            public static readonly Error NotFound = new(
                "Payment.NotFound",
                "The payment transaction was not found.");

            public static readonly Error AlreadyProcessed = new(
                "Payment.AlreadyProcessed",
                "This payment transaction has already been processed.");

            public static readonly Error Failed = new(
                "Payment.Failed",
                "The payment transaction failed.");

            public static readonly Error InvalidSignature = new(
                "Payment.InvalidSignature",
                "The payment webhook signature is invalid.");
        }
    }
}


