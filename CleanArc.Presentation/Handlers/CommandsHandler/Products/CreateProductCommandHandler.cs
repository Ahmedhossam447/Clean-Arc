using CleanArc.Application.Commands.Product;
using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Products
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IUnitOfWork unitOfWork,
            IImageService imageService,
            ILogger<CreateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            string? imageUrl = null;

            if (request.Image != null)
            {
                imageUrl = await _imageService.UploadImageAsync(request.Image, request.FileName);

                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("Image upload returned empty URL for file: {FileName}", request.FileName);
                    return Product.Errors.PhotoUploadFailed;
                }
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = imageUrl,
                ShelterId = request.ShelterId
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateProductResponse
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
