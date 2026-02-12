using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.Product
{
    public class GetProductByIdQuery : IRequest<Result<ReadProductResponse>>
    {
        [JsonIgnore]
        public int ProductId { get; set; }
    }
}
