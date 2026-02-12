using CleanArc.Application.Commands.Product;
using CleanArc.Application.Handlers.CommandsHandler.Products;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.ProductTests
{
    public class DeleteProductHandlerTests
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IBackgroundJobService _BackgroundJobService;
        private readonly ILogger<DeleteProductCommandHandler> _Logger;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductHandlerTests()
        {
            _UnitOfWork = Substitute.For<IUnitOfWork>();
            _BackgroundJobService = Substitute.For<IBackgroundJobService>();
            _Logger = Substitute.For<ILogger<DeleteProductCommandHandler>>();
            _handler = new DeleteProductCommandHandler(_UnitOfWork, _BackgroundJobService, _Logger);
        }

        [Fact]
        public async Task DeleteProductHandler_Should_DeleteProduct_When_OwnerDeletes()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Cat Food",
                ShelterId = "shelter-123",
                ImageUrl = "https://s3.amazonaws.com/bucket/catfood.jpg"
            };
            var command = new DeleteProductCommand { ProductId = 1, UserId = "shelter-123" };

            _UnitOfWork.Repository<Product>().MockGetProductByIdAsync(product);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.Message.Should().Contain("deleted");

            _BackgroundJobService.Received(1).EnqueueJob<IImageService>(Arg.Any<Expression<Func<IImageService, Task>>>());
            await _UnitOfWork.Repository<Product>().Received(1).Delete(1);
            await _UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteProductHandler_Should_Fail_When_ProductNotFound()
        {
            var command = new DeleteProductCommand { ProductId = 999, UserId = "shelter-123" };

            _UnitOfWork.Repository<Product>().MockGetProductByIdReturnsNull(999);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Product.NotFound");

            await _UnitOfWork.Repository<Product>().DidNotReceive().Delete(Arg.Any<int>());
            await _UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteProductHandler_Should_Fail_When_UserIsNotOwner()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Cat Food",
                ShelterId = "shelter-123",
                ImageUrl = "https://s3.amazonaws.com/bucket/catfood.jpg"
            };
            var command = new DeleteProductCommand { ProductId = 1, UserId = "other-user-456" };

            _UnitOfWork.Repository<Product>().MockGetProductByIdAsync(product);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Product.Unauthorized");

            await _UnitOfWork.Repository<Product>().DidNotReceive().Delete(Arg.Any<int>());
            await _UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteProductHandler_Should_NotEnqueueJob_When_NoImage()
        {
            var product = new Product
            {
                Id = 2,
                Name = "Dog Bowl",
                ShelterId = "shelter-123",
                ImageUrl = null
            };
            var command = new DeleteProductCommand { ProductId = 2, UserId = "shelter-123" };

            _UnitOfWork.Repository<Product>().MockGetProductByIdAsync(product);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _BackgroundJobService.DidNotReceive().EnqueueJob<IImageService>(Arg.Any<Expression<Func<IImageService, Task>>>());
        }
    }
}
