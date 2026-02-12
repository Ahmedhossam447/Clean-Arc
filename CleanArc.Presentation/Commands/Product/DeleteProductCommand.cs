using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Product
{
    public class DeleteProductCommand : IRequest<Result<DeleteProductResponse>>
    {
        [JsonIgnore]
        public int ProductId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }
}
