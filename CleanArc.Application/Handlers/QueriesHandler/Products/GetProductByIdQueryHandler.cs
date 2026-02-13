using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Application.Queries.Product;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Products
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ReadProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReadProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return Product.Errors.NotFound;
            }

            var response = new ReadProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                ShelterId = product.ShelterId
            };

            return response;
        }
    }
}
