using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Product;
using MediatR;

namespace CleanArc.Application.Queries.Product
{
    public class GetAllProductsQuery : IRequest<PaginationResponse<ReadProductResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
