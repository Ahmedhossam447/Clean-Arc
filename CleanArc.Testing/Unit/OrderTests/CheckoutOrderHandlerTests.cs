using CleanArc.Application.Commands.Order;
using CleanArc.Application.Handlers.CommandsHandler.Orders;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CleanArc.Testing.Unit.OrderTests
{
    public class CheckoutOrderHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;
        private readonly CheckoutOrderCommandHandler _handler;

        public CheckoutOrderHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _paymentService = Substitute.For<IPaymentService>();
            
            // Setup generic IConfiguration 
            _config = Substitute.For<IConfiguration>();
            var configSection = Substitute.For<IConfigurationSection>();
            configSection.Value.Returns("mock_iframe_id");
            _config.GetSection("Paymob:IframeId").Returns(configSection);
            _config["Paymob:IframeId"].Returns("mock_iframe_id");

            _logger = Substitute.For<ILogger<CheckoutOrderCommandHandler>>();
            _orderRepository = Substitute.For<IRepository<Order>>();
            _productRepository = Substitute.For<IRepository<Product>>();
            _paymentTransactionRepository = Substitute.For<IRepository<PaymentTransaction>>();

            _unitOfWork.Repository<Order>().Returns(_orderRepository);
            _unitOfWork.Repository<Product>().Returns(_productRepository);
            _unitOfWork.Repository<PaymentTransaction>().Returns(_paymentTransactionRepository);

            _handler = new CheckoutOrderCommandHandler(_unitOfWork, _paymentService, _config, _logger);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };

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
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user2", Status = "Pending" };

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
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Processing" };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.AlreadyProcessed);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenOrderIsEmpty()
        {
            // Arrange
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order { Id = 1, BuyerId = "user1", Status = "Pending", OrderItems = new List<OrderItem>() };

            _orderRepository.MockGetOrderWithItemsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Order.Errors.EmptyOrder);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } }
            };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdReturnsNull(1);

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
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 5 } }
            };
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
        public async Task Handle_Should_RollbackAndReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1" };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1, Price = 100m } }
            };
            var product = new Product { Id = 1, StockQuantity = 10 };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdAsync(product);

            // Throw exception when trying to contact Paymob
            _paymentService.GetAuthTokenAsync().Throws(new Exception("Paymob down"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(PaymentTransaction.Errors.Failed);
            
            await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_CommitTransactionAndReturnPaymentUrl_WhenSuccessful()
        {
            // Arrange
            var command = new CheckoutOrderCommand { OrderId = 1, UserId = "user1", UserEmail = "user@test.com" };
            var order = new Order 
            { 
                Id = 1, 
                BuyerId = "user1", 
                Status = "Pending", 
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2, Price = 50m } }
            };
            var product = new Product { Id = 1, StockQuantity = 10 };

            _orderRepository.MockGetOrderWithItemsAsync(order);
            _productRepository.MockGetProductByIdAsync(product);

            _paymentService.MockGetAuthTokenAsync("mock_auth_token");
            _paymentService.MockCreateOrderAsync(123456);
            _paymentService.MockGetPaymentKeyAsync("mock_payment_key");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.OrderId.Should().Be(1);
            result.Value.Subtotal.Should().Be(100m); // 2 * 50
            result.Value.PaymentUrl.Should().Be("https://accept.paymob.com/api/acceptance/iframes/mock_iframe_id?payment_token=mock_payment_key");

            await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
            await _paymentTransactionRepository.Received(1).AddAsync(Arg.Is<PaymentTransaction>(pt => pt.Amount == 100m && pt.Status == "Pending"));
            _orderRepository.Received(1).Update(Arg.Is<Order>(o => o.Subtotal == 100m));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        }
    }
}
