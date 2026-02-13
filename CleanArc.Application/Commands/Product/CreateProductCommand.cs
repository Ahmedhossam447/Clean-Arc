using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Product
{
    public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        [JsonIgnore]
        public Stream? Image { get; set; }
        [JsonIgnore]
        public string FileName { get; set; } = string.Empty;

        [JsonIgnore]
        public string ShelterId { get; set; }
    }
}
