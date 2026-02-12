using CleanArc.Application.Commands.Product;
using CleanArc.Application.Handlers.CommandsHandler.Products;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CleanArc.Testing.Unit.ProductTests
{
    public class CreateProductHandlerTests
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IImageService _ImageService;
        private readonly ILogger<CreateProductCommandHandler> _Logger;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductHandlerTests()
        {
            _UnitOfWork = Substitute.For<IUnitOfWork>();
            _ImageService = Substitute.For<IImageService>();
            _Logger = Substitute.For<ILogger<CreateProductCommandHandler>>();
            _handler = new CreateProductCommandHandler(_UnitOfWork, _ImageService, _Logger);
        }

        [Fact]
        public async Task CreateProductHandler_Should_CreateProduct_When_ValidCommandWithPhoto()
        {
            var command = new CreateProductCommand
            {
                Name = "Cat Food",
                Description = "Premium cat food",
                Price = 99.99m,
                StockQuantity = 50,
                ShelterId = "shelter-123",
                Image = new MemoryStream(new byte[] { 1, 2, 3 }),
                FileName = "catfood.jpg"
            };

            _ImageService.UploadImageAsync(Arg.Any<Stream>(), Arg.Any<string>())
                .Returns("https://s3.amazonaws.com/bucket/catfood.jpg");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Cat Food");
            result.Value.Price.Should().Be(99.99m);
            result.Value.StockQuantity.Should().Be(50);
            result.Value.ImageUrl.Should().Be("https://s3.amazonaws.com/bucket/catfood.jpg");
            result.Value.ShelterId.Should().Be("shelter-123");

            await _UnitOfWork.Repository<Product>().Received(1).AddAsync(Arg.Any<Product>());
            await _UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateProductHandler_Should_CreateProduct_When_NoPhotoProvided()
        {
            var command = new CreateProductCommand
            {
                Name = "Dog Toy",
                Description = "Squeaky toy",
                Price = 25.00m,
                StockQuantity = 100,
                ShelterId = "shelter-456",
                Image = null,
                FileName = string.Empty
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Dog Toy");
            result.Value.ImageUrl.Should().BeNull();

            await _ImageService.DidNotReceive().UploadImageAsync(Arg.Any<Stream>(), Arg.Any<string>());
            await _UnitOfWork.Repository<Product>().Received(1).AddAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task CreateProductHandler_Should_Fail_When_PhotoUploadReturnsEmpty()
        {
            var command = new CreateProductCommand
            {
                Name = "Leash",
                Description = "Dog leash",
                Price = 15.00m,
                StockQuantity = 30,
                ShelterId = "shelter-789",
                Image = new MemoryStream(new byte[] { 1 }),
                FileName = "leash.png"
            };

            _ImageService.UploadImageAsync(Arg.Any<Stream>(), Arg.Any<string>())
                .Returns(string.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Product.PhotoUploadFailed");

            await _UnitOfWork.Repository<Product>().DidNotReceive().AddAsync(Arg.Any<Product>());
            await _UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
