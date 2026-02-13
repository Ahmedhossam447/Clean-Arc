using CleanArc.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace CleanArc.Infrastructure.Services
{
    public class PaymobService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ApiKey;
        private readonly string _IntegrationId;

        public PaymobService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _ApiKey = config["Paymob:ApiKey"]
                ?? throw new InvalidOperationException("Paymob:ApiKey not configured");
            _IntegrationId = config["Paymob:IntegrationId"]
                ?? throw new InvalidOperationException("Paymob:IntegrationId not configured");
        }

        public async Task<int> CreateOrderAsync(string token, decimal AmountCents)
        {
            var requestbody = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = AmountCents.ToString(),
                currency = "EGP",
                items = new object[] { }
            };
            var response =await _httpClient.PostAsJsonAsync("ecommerce/orders", requestbody);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            return data.GetProperty("id").GetInt32();
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("auth/tokens", new { api_key = _ApiKey });
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            return data.GetProperty("token").GetString();
        }

        public async Task<string> GetPaymentKeyAsync(string token, int orderId, decimal amountCents, object billingData)
        {
            var requestbody = new
            {
                auth_token = token,
                amount_cents = amountCents.ToString(),
                expiration = 3600, // 1 hour
                order_id = orderId,
                billing_data = billingData,
                currency = "EGP",
                integration_id = _IntegrationId
            };
            var response =await _httpClient.PostAsJsonAsync("acceptance/payment_keys", requestbody);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            return data.GetProperty("token").GetString();
        }
    }
}
