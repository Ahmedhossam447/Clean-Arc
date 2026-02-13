using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Product
{
    public class UpdateProductCommand : IRequest<Result<UpdateProductResponse>>
    {
        [JsonIgnore]
        public int ProductId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }

        [JsonIgnore]
        public Stream? Image { get; set; }
        [JsonIgnore]
        public string FileName { get; set; } = string.Empty;
    }
}
