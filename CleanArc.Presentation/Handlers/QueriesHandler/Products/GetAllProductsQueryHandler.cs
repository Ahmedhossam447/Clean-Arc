using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Application.Queries.Product;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Products
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginationResponse<ReadProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<ReadProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync(cancellationToken);
            int totalCount = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var pagedProducts = products
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var response = new PaginationResponse<ReadProductResponse>
            {
                Items = pagedProducts.Select(p => new ReadProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    ShelterId = p.ShelterId
                }).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };

            return response;
        }
    }
}
