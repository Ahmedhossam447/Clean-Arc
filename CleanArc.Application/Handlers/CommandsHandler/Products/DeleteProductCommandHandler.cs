using CleanArc.Application.Commands.Product;
using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Products
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<DeleteProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IUnitOfWork unitOfWork,
            IBackgroundJobService backgroundJobService,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _backgroundJobService = backgroundJobService;
            _logger = logger;
        }

        public async Task<Result<DeleteProductResponse>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.Repository<Product>();

            var product = await productRepo.GetByIdAsync(command.ProductId, cancellationToken);
            if (product == null)
            {
                return Product.Errors.NotFound;
            }

            // Authorization check: user can only delete their own products
            if (product.ShelterId != command.UserId)
            {
                return Product.Errors.Unauthorized;
            }

            // Queue image deletion via background job
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                _backgroundJobService.EnqueueJob<IImageService>(x => x.DeleteImageAsync(product.ImageUrl));
            }

            await productRepo.Delete(product.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeleteProductResponse
            {
                Id = command.ProductId,
                Message = "Product deleted successfully."
            };
        }
    }
}
