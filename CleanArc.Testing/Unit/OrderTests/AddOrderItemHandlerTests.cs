using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Application.Handlers.CommandsHandler.Orders;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.OrderTests
{
    public class AddOrderItemHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly AddOrderItemCommandHandler _handler;

        public AddOrderItemHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderRepository = Substitute.For<IRepository<Order>>();
            _productRepository = Substitute.For<IRepository<Product>>();
            _orderItemRepository = Substitute.For<IRepository<OrderItem>>();

            _unitOfWork.Repository<Order>().Returns(_orderRepository);
            _unitOfWork.Repository<Product>().Returns(_productRepository);
            _unitOfWork.Repository<OrderItem>().Returns(_orderItemRepository);

            _handler = new AddOrderItemCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            
            _orderRepository.MockGetOrderWithItemsReturnsEmpty();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUserIsNotBuyer()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user2", Status = "Pending" }; // Different buyer

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderStatusIsNotPending()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Processing" };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.AlreadyProcessed);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Pending" };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdReturnsNull(command.ProductId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Product.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenInsufficientStock()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 5, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Pending" };
            var product = new Product { Id = 1, StockQuantity = 2 }; // Not enough stock

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.InsufficientStock);
        }

        [Fact]
        public async Task Handle_Should_AddNewItem_WhenProductNotInOrder()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Pending", OrderItems = new List<OrderItem>() };
            var product = new Product { Id = 1, Name = "Food", Price = 10m, StockQuantity = 10 };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.ProductId.Should().Be(product.Id);
            result.Value.Quantity.Should().Be(2);

            await _orderItemRepository.Received(1).AddAsync(Arg.Is<OrderItem>(oi => oi.ProductId == product.Id && oi.Quantity == 2));
            _orderRepository.Received(1).Update(Arg.Is<Order>(o => o.Subtotal == 20m));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_IncreaseQuantity_WhenProductAlreadyInOrder()
        {
            // Arrange
            var command = new AddOrderItemCommand { OrderId = 1, ProductId = 1, Quantity = 2, UserId = "user1" };
            var existingItem = new OrderItem { ProductId = 1, Quantity = 3, Price = 10m };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                OrderItems = new List<OrderItem> { existingItem } 
            };
            var product = new Product { Id = 1, Name = "Food", Price = 10m, StockQuantity = 10 };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Quantity.Should().Be(5); // 3 existing + 2 new

            await _orderItemRepository.DidNotReceiveWithAnyArgs().AddAsync(default!);
            _orderItemRepository.Received(1).Update(Arg.Is<OrderItem>(oi => oi.Quantity == 5));
            _orderRepository.Received(1).Update(Arg.Is<Order>(o => o.Subtotal == 50m)); // 5 * 10
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
