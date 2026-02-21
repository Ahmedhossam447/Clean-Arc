using CleanArc.Application.Commands.Order;
using CleanArc.Application.Handlers.CommandsHandler.Orders;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using NSubstitute;

namespace CleanArc.Testing.Unit.OrderTests
{
    public class UpdateOrderItemStatusHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly UpdateOrderItemStatusCommandHandler _handler;

        public UpdateOrderItemStatusHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderRepository = Substitute.For<IRepository<Order>>();
            _orderItemRepository = Substitute.For<IRepository<OrderItem>>();

            _unitOfWork.Repository<Order>().Returns(_orderRepository);
            _unitOfWork.Repository<OrderItem>().Returns(_orderItemRepository);

            _handler = new UpdateOrderItemStatusCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenStatusIsInvalid()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 1, ShelterId = "shelter1", Status = "InvalidStatus" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("OrderItem.InvalidStatus");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 1, ShelterId = "shelter1", Status = "Shipped" };

            _orderRepository.MockGetOrderWithItemsReturnsEmpty();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderIsNotPaid()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 1, ShelterId = "shelter1", Status = "Shipped" };
            var order = new Order { Id = 1, Status = "Pending" }; // Not PaymentReceived

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order.NotPaid");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenItemNotFoundInOrder()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 99, ShelterId = "shelter1", Status = "Shipped" };
            var existingItem = new OrderItem { Id = 1, ShelterId = "shelter1" };
            var order = new Order 
            { 
                Id = 1, 
                Status = "PaymentReceived",
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
        public async Task Handle_Should_ReturnFailure_WhenShelterIsNotOwner()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 1, ShelterId = "shelter_thief", Status = "Shipped" };
            var existingItem = new OrderItem { Id = 1, ShelterId = "shelter_owner" };
            var order = new Order 
            { 
                Id = 1, 
                Status = "PaymentReceived",
                OrderItems = new List<OrderItem> { existingItem } 
            };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_UpdateStatus_WhenAllValidationsPass()
        {
            // Arrange
            var command = new UpdateOrderItemStatusCommand { OrderId = 1, ItemId = 1, ShelterId = "shelter_owner", Status = "Delivered" };
            var existingItem = new OrderItem { Id = 1, ShelterId = "shelter_owner", Status = "Processing" };
            var order = new Order 
            { 
                Id = 1, 
                Status = "PaymentReceived",
                OrderItems = new List<OrderItem> { existingItem } 
            };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            _orderItemRepository.Received(1).Update(Arg.Is<OrderItem>(oi => oi.Status == "Delivered"));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
