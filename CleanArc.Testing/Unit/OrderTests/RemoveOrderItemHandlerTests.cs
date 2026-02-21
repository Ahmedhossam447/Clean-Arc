using CleanArc.Application.Commands.Order;
using CleanArc.Application.Handlers.CommandsHandler.Orders;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using NSubstitute;

namespace CleanArc.Testing.Unit.OrderTests
{
    public class RemoveOrderItemHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly RemoveOrderItemCommandHandler _handler;

        public RemoveOrderItemHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderRepository = Substitute.For<IRepository<Order>>();
            _orderItemRepository = Substitute.For<IRepository<OrderItem>>();

            _unitOfWork.Repository<Order>().Returns(_orderRepository);
            _unitOfWork.Repository<OrderItem>().Returns(_orderItemRepository);

            _handler = new RemoveOrderItemCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new RemoveOrderItemCommand { OrderId = 1, ItemId = 1, UserId = "user1" };

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
            var command = new RemoveOrderItemCommand { OrderId = 1, ItemId = 1, UserId = "user1" };
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
            var command = new RemoveOrderItemCommand { OrderId = 1, ItemId = 1, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Processing" };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.AlreadyProcessed);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenItemNotFoundInOrder()
        {
            // Arrange
            var command = new RemoveOrderItemCommand { OrderId = 1, ItemId = 99, UserId = "user1" };
            var existingItem = new OrderItem { Id = 1, ProductId = 1, Quantity = 2, Price = 10m };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending",
                OrderItems = new List<OrderItem> { existingItem } 
            };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("OrderItem.NotFound");
        }

        [Fact]
        public async Task Handle_Should_RemoveItemAndRecalculateSubtotal_WhenItemExists()
        {
            // Arrange
            var command = new RemoveOrderItemCommand { OrderId = 1, ItemId = 2, UserId = "user1" };
            var itemToKeep = new OrderItem { Id = 1, Quantity = 2, Price = 10m }; // Subtotal = 20
            var itemToRemove = new OrderItem { Id = 2, Quantity = 1, Price = 5m }; // Subtotal = +5
            
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                Subtotal = 25m, // Initial subtotal
                OrderItems = new List<OrderItem> { itemToKeep, itemToRemove } 
            };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            await _orderItemRepository.Received(1).Delete(itemToRemove.Id);
            _orderRepository.Received(1).Update(Arg.Is<Order>(o => o.Subtotal == 20m)); // Remaining subtotal recalculation verification
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
