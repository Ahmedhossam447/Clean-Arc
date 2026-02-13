using CleanArc.Application.Commands.Product;
using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Products
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IUnitOfWork unitOfWork,
            IImageService imageService,
            IBackgroundJobService backgroundJobService,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _backgroundJobService = backgroundJobService;
            _logger = logger;
        }

        public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.Repository<Product>();

            var product = await productRepo.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return Product.Errors.NotFound;
            }

            // Authorization check: user can only update their own products
            if (product.ShelterId != request.UserId)
            {
                return Product.Errors.Unauthorized;
            }

            string? oldImageUrl = null;

            // Handle image update: delete old image if new one is provided
            if (request.Image != null && !string.IsNullOrEmpty(request.FileName))
            {
                var newImageUrl = await _imageService.UploadImageAsync(request.Image, request.FileName);
                if (string.IsNullOrEmpty(newImageUrl))
                {
                    _logger.LogWarning("Image upload returned empty URL for product {ProductId}", request.ProductId);
                    return Product.Errors.PhotoUploadFailed;
                }

                // Store old URL for background deletion
                oldImageUrl = product.ImageUrl;
                product.ImageUrl = newImageUrl;
            }

            product.Name = request.Name ?? product.Name;
            product.Description = request.Description ?? product.Description;
            product.Price = request.Price ?? product.Price;
            product.StockQuantity = request.StockQuantity ?? product.StockQuantity;

            productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Queue old image deletion after successful save
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                _backgroundJobService.EnqueueJob<IImageService>(x => x.DeleteImageAsync(oldImageUrl));
            }

            var response = new UpdateProductResponse
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
