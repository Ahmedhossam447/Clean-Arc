using CleanArc.Application.Commands.Payment;
using CleanArc.Application.Handlers.CommandsHandler.Payment;
using CleanArc.Core.Dtos;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CleanArc.Testing.Unit.PaymentTests
{
    public class ProcessWebhookHandlerTests
    {
        private readonly IPaymobSecurity _PaymobSecurity;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ILogger<ProcessPaymobWebhookCommandHandler> _Logger;
        private readonly ProcessPaymobWebhookCommandHandler _handler;

        public ProcessWebhookHandlerTests()
        {
            _PaymobSecurity = Substitute.For<IPaymobSecurity>();
            _UnitOfWork = Substitute.For<IUnitOfWork>();
            _Logger = Substitute.For<ILogger<ProcessPaymobWebhookCommandHandler>>();
            _handler = new ProcessPaymobWebhookCommandHandler(_PaymobSecurity, _UnitOfWork, _Logger);
        }

        private static PaymobWebhookDto CreateWebhookDto(bool success, int paymobOrderId, string hmac = "valid-hmac")
        {
            return new PaymobWebhookDto
            {
                Type = "TRANSACTION",
                Hmac = hmac,
                Obj = new PaymobTransactionObj
                {
                    Id = 1001,
                    Success = success,
                    Pending = false,
                    AmountCents = 10000,
                    IsAuth = false,
                    IsCapture = false,
                    IsVoided = false,
                    IsRefunded = false,
                    Is3dSecure = true,
                    IntegrationId = 123,
                    ProfileId = 456,
                    HasParentTransaction = false,
                    CreatedAt = "2026-01-01T00:00:00",
                    Currency = "EGP",
                    ErrorOccured = false,
                    IsStandalonePayment = true,
                    Owner = 789,
                    Order = new PaymobOrderObj { Id = paymobOrderId },
                    SourceData = new PaymobSourceData { Type = "card", Pan = "1234", SubType = "Visa" }
                }
            };
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_Fail_When_InvalidHmac()
        {
            var webhook = CreateWebhookDto(true, 100, "bad-hmac");
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Payment.InvalidSignature");
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_Fail_When_PaymentNotFound()
        {
            var webhook = CreateWebhookDto(true, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentReturnsEmpty();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Payment.NotFound");
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_Fail_When_AlreadyProcessed()
        {
            var payment = new PaymentTransaction { Id = 1, PaymobOrderId = 100, Status = "Successful" };
            var webhook = CreateWebhookDto(true, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentByPaymobOrderId(payment);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Payment.AlreadyProcessed");
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_Succeed_When_PaymentSuccessAndStockAvailable()
        {
            var payment = new PaymentTransaction { Id = 1, PaymobOrderId = 100, Status = "Pending" };
            var order = new Order { Id = 10, PaymentTransactionId = 1, Status = "Pending", BuyerId = "user-1" };
            var orderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 10, ProductId = 1, Quantity = 2, ProductName = "Cat Food", Price = 50m, ShelterId = "s-1" },
                new OrderItem { Id = 2, OrderId = 10, ProductId = 2, Quantity = 1, ProductName = "Dog Toy", Price = 25m, ShelterId = "s-2" }
            };
            var webhook = CreateWebhookDto(true, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentByPaymobOrderId(payment);
            _UnitOfWork.Repository<Order>().MockGetOrderByPaymentId(order);
            _UnitOfWork.Repository<OrderItem>().MockGetOrderItems(orderItems);
            _UnitOfWork.ExecuteSqlRawAsync(Arg.Any<string>(), Arg.Any<object[]>()).Returns(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
            payment.Status.Should().Be("Successful");
            order.Status.Should().Be("PaymentReceived");

            await _UnitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
            await _UnitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
            // 2 items = 2 SQL calls
            await _UnitOfWork.Received(2).ExecuteSqlRawAsync(Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_RollbackAndMarkFailed_When_InsufficientStock()
        {
            var payment = new PaymentTransaction { Id = 1, PaymobOrderId = 100, Status = "Pending" };
            var order = new Order { Id = 10, PaymentTransactionId = 1, Status = "Pending", BuyerId = "user-1" };
            var orderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 10, ProductId = 1, Quantity = 2, ProductName = "Cat Food", Price = 50m, ShelterId = "s-1" },
                new OrderItem { Id = 2, OrderId = 10, ProductId = 2, Quantity = 5, ProductName = "Dog Toy", Price = 25m, ShelterId = "s-2" }
            };
            var webhook = CreateWebhookDto(true, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentByPaymobOrderId(payment);
            _UnitOfWork.Repository<Order>().MockGetOrderByPaymentId(order);
            _UnitOfWork.Repository<OrderItem>().MockGetOrderItems(orderItems);

            // First item succeeds, second item fails (0 rows affected = stock insufficient)
            _UnitOfWork.ExecuteSqlRawAsync(Arg.Any<string>(), Arg.Any<object[]>())
                .Returns(1, 0);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order.InsufficientStock");
            payment.Status.Should().Be("Failed");
            order.Status.Should().Be("StockUnavailable");

            await _UnitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_MarkPaymentFailed_When_PaymentNotSuccessful()
        {
            var payment = new PaymentTransaction { Id = 1, PaymobOrderId = 100, Status = "Pending" };
            var failedOrder = new Order { Id = 10, PaymentTransactionId = 1, Status = "Pending", BuyerId = "user-1" };
            var webhook = CreateWebhookDto(false, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentByPaymobOrderId(payment);
            _UnitOfWork.Repository<Order>().MockGetOrderByPaymentId(failedOrder);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Payment.Failed");
            payment.Status.Should().Be("Failed");
            failedOrder.Status.Should().Be("PaymentFailed");

            await _UnitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task ProcessWebhookHandler_Should_ProcessItems_InProductIdOrder()
        {
            var payment = new PaymentTransaction { Id = 1, PaymobOrderId = 100, Status = "Pending" };
            var order = new Order { Id = 10, PaymentTransactionId = 1, Status = "Pending", BuyerId = "user-1" };
            // Items returned out of order
            var orderItems = new List<OrderItem>
            {
                new OrderItem { Id = 2, OrderId = 10, ProductId = 5, Quantity = 1, ProductName = "Product 5", Price = 10m, ShelterId = "s-1" },
                new OrderItem { Id = 1, OrderId = 10, ProductId = 1, Quantity = 1, ProductName = "Product 1", Price = 20m, ShelterId = "s-1" },
                new OrderItem { Id = 3, OrderId = 10, ProductId = 3, Quantity = 1, ProductName = "Product 3", Price = 15m, ShelterId = "s-1" }
            };
            var webhook = CreateWebhookDto(true, 100);
            var command = new ProcessPaymobWebhookCommand { PaymobWebhook = webhook };

            _PaymobSecurity.ValidateHmac(Arg.Any<PaymobTransactionObj>(), Arg.Any<string>()).Returns(true);
            _UnitOfWork.Repository<PaymentTransaction>().MockGetPaymentByPaymobOrderId(payment);
            _UnitOfWork.Repository<Order>().MockGetOrderByPaymentId(order);
            _UnitOfWork.Repository<OrderItem>().MockGetOrderItems(orderItems);

            var executedProductIds = new List<int>();
            _UnitOfWork.ExecuteSqlRawAsync(Arg.Any<string>(), Arg.Any<object[]>())
                .ReturnsForAnyArgs(callInfo =>
                {
                    var args = callInfo.ArgAt<object[]>(1);
                    executedProductIds.Add((int)args[1]); // second param is ProductId
                    return Task.FromResult(1);
                });

            await _handler.Handle(command, CancellationToken.None);

            // Should be sorted: 1, 3, 5
            executedProductIds.Should().BeInAscendingOrder();
            executedProductIds.Should().Equal(1, 3, 5);
        }
    }
}
