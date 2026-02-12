using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Requests.Order;
using CleanArc.Application.Handlers.CommandsHandler.Orders;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using NSubstitute;

namespace CleanArc.Testing.Unit.OrderTests
{
    public class CreateOrderHandlerTests
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderHandlerTests()
        {
            _UnitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateOrderCommandHandler(_UnitOfWork);
        }

        [Fact]
        public async Task CreateOrderHandler_Should_Fail_When_ItemsEmpty()
        {
            var command = new CreateOrderCommand
            {
                Items = new List<CartItemRequest>(),
                CustomerId = "user-123",
                CustomerEmail = "user@test.com"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order.EmptyOrder");
        }

        [Fact]
        public async Task CreateOrderHandler_Should_Fail_When_ItemsNull()
        {
            var command = new CreateOrderCommand
            {
                Items = null!,
                CustomerId = "user-123",
                CustomerEmail = "user@test.com"
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order.EmptyOrder");
        }

        [Fact]
        public async Task CreateOrderHandler_Should_Fail_When_ProductNotFound()
        {
            var command = new CreateOrderCommand
            {
                Items = new List<CartItemRequest>
                {
                    new CartItemRequest { ProductId = 999, Quantity = 1 }
                },
                CustomerId = "user-123",
                CustomerEmail = "user@test.com"
            };

            _UnitOfWork.Repository<Product>().MockGetProductByIdReturnsNull(999);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Product.NotFound");
        }

        [Fact]
        public async Task CreateOrderHandler_Should_Fail_When_InsufficientStock()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Cat Food",
                Price = 50.00m,
                StockQuantity = 2,
                ShelterId = "shelter-1",
                ImageUrl = "img.jpg"
            };

            var command = new CreateOrderCommand
            {
                Items = new List<CartItemRequest>
                {
                    new CartItemRequest { ProductId = 1, Quantity = 10 }
                },
                CustomerId = "user-123",
                CustomerEmail = "user@test.com"
            };

            _UnitOfWork.Repository<Product>().MockGetProductByIdAsync(product);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order.InsufficientStock");
        }

        [Fact]
        public async Task CreateOrderHandler_Should_MergeDuplicateProductIds()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Cat Food",
                Price = 50.00m,
                StockQuantity = 10,
                ShelterId = "shelter-1",
                ImageUrl = "img.jpg"
            };

            var command = new CreateOrderCommand
            {
                Items = new List<CartItemRequest>
                {
                    new CartItemRequest { ProductId = 1, Quantity = 3 },
                    new CartItemRequest { ProductId = 1, Quantity = 2 }
                },
                CustomerId = "user-123",
                CustomerEmail = "user@test.com"
            };

            _UnitOfWork.Repository<Product>().MockGetProductByIdAsync(product);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            // Merged: 3 + 2 = 5, so subtotal = 50 * 5 = 250
            result.Value.Subtotal.Should().Be(250.00m);
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items[0].Quantity.Should().Be(5);
        }

        [Fact]
        public async Task CreateOrderHandler_Should_CreateOrder_When_ValidCommand()
        {
            var product1 = new Product { Id = 1, Name = "Cat Food", Price = 50.00m, StockQuantity = 10, ShelterId = "s-1", ImageUrl = "img1.jpg" };
            var product2 = new Product { Id = 2, Name = "Dog Toy", Price = 25.00m, StockQuantity = 20, ShelterId = "s-2", ImageUrl = "img2.jpg" };

            var command = new CreateOrderCommand
            {
                Items = new List<CartItemRequest>
                {
                    new CartItemRequest { ProductId = 1, Quantity = 2 },
                    new CartItemRequest { ProductId = 2, Quantity = 3 }
                },
                CustomerId = "user-123",
                CustomerEmail = "buyer@test.com"
            };

            var productRepo = _UnitOfWork.Repository<Product>();
            productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product1);
            productRepo.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(product2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            // (50 * 2) + (25 * 3) = 100 + 75 = 175
            result.Value.Subtotal.Should().Be(175.00m);
            result.Value.Currency.Should().Be("EGP");
            result.Value.Status.Should().Be("Pending");
            result.Value.Items.Should().HaveCount(2);

            await _UnitOfWork.Repository<Order>().Received(1).AddAsync(Arg.Any<Order>());
            await _UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
