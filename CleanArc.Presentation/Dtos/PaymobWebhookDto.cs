using System.Text.Json.Serialization;

namespace CleanArc.Application.Dtos
{
    public class PaymobWebhookDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("obj")]
        public PaymobTransactionObj Obj { get; set; }

        [JsonIgnore]
        public string Hmac { get; set; }
    }

    public class PaymobTransactionObj
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; set; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_3d_secure")]
        public bool Is3dSecure { get; set; }

        [JsonPropertyName("integration_id")]
        public int IntegrationId { get; set; }

        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonPropertyName("order")]
        public PaymobOrderObj Order { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonPropertyName("owner")]
        public int Owner { get; set; }

        [JsonPropertyName("source_data")]
        public PaymobSourceData SourceData { get; set; }
    }

    public class PaymobOrderObj
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class PaymobSourceData
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("sub_type")]
        public string SubType { get; set; }
    }
}