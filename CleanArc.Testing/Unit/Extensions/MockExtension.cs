using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.Extensions
{
    public static class MockExtension
    {
        // --- Animal ---
        public static void MockGetAnimalByIdAsync(this IRepository<Animal> repository, Animal animal)
        {
            repository.GetByIdAsync(animal.Id).Returns(Task.FromResult<Animal?>(animal));
        }

        public static void MockGetAnimalByIdReturnsNull(this IRepository<Animal> repository, int id)
        {
            repository.GetByIdAsync(id).Returns(Task.FromResult<Animal?>(null));
        }

        // --- Product ---
        public static void MockGetProductByIdAsync(this IRepository<Product> repository, Product product)
        {
            repository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(product));
        }

        public static void MockGetProductByIdReturnsNull(this IRepository<Product> repository, int id)
        {
            repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product?>(null));
        }

        // --- PaymentTransaction ---
        public static void MockGetPaymentByPaymobOrderId(this IRepository<PaymentTransaction> repository, PaymentTransaction payment)
        {
            repository.GetAsync(Arg.Any<Expression<Func<PaymentTransaction, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<PaymentTransaction>>(new List<PaymentTransaction> { payment }));
        }

        public static void MockGetPaymentReturnsEmpty(this IRepository<PaymentTransaction> repository)
        {
            repository.GetAsync(Arg.Any<Expression<Func<PaymentTransaction, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<PaymentTransaction>>(new List<PaymentTransaction>()));
        }

        // --- Order ---
        public static void MockGetOrderByPaymentId(this IRepository<Order> repository, Order order)
        {
            repository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<Order>>(new List<Order> { order }));
        }

        public static void MockGetOrderReturnsEmpty(this IRepository<Order> repository)
        {
            repository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<Order>>(new List<Order>()));
        }

        // --- OrderItem ---
        public static void MockGetOrderItems(this IRepository<OrderItem> repository, List<OrderItem> items)
        {
            repository.GetAsync(Arg.Any<Expression<Func<OrderItem, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<OrderItem>>(items));
        }

        // --- Request ---
        public static void MockGetRequestsByAnimalId(this IRepository<Request> repository, List<Request> requests)
        {
            repository.GetAsync(Arg.Any<Expression<Func<Request, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<Request>>(requests));
        }

        // --- User ---
        public static void MockGetUserByIdAsync(this IUserService userService, AuthUser user)
        {
            userService.GetUserByIdAsync(Arg.Is(user.Id)).Returns(Task.FromResult(user));
        }

        public static void MockGetUserByIdAsync(this IUserService userService, string id, string name)
        {
            userService.MockGetUserByIdAsync(new AuthUser { Id = id, UserName = name, Email = $"{name}@test.com" });
        }
    }
}