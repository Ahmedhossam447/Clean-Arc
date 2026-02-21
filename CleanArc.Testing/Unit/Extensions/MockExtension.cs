using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Entities;
using CleanArc.Core.Models.Identity;
using CleanArc.Core.Primitives;
using Microsoft.Extensions.Caching.Distributed;
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

        public static void MockGetOrderWithItemsAsync(this IRepository<Order> repository, Order order)
        {
            repository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Order, object>>>())
                .Returns(Task.FromResult<IEnumerable<Order>>(new List<Order> { order }));
        }

        public static void MockGetOrderWithItemsReturnsEmpty(this IRepository<Order> repository)
        {
            repository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Order, object>>>())
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

        public static void MockGetUserByEmailAsync(this IUserService userService, params AuthUser?[] returnedUsers)
        {
            if (returnedUsers.Length == 1)
            {
                userService.GetUserByEmailAsync(Arg.Any<string>()).Returns(returnedUsers[0]);
            }
            else if (returnedUsers.Length > 1)
            {
                userService.GetUserByEmailAsync(Arg.Any<string>()).Returns(returnedUsers[0], returnedUsers.Skip(1).ToArray());
            }
        }

        // --- GoogleAuthService ---
        public static void MockValidateTokenAsync(this IGoogleAuthService googleAuthService, Result<GoogleUser> result)
        {
            googleAuthService.ValidateTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(result);
        }

        // --- AuthService ---
        public static void MockLoginUserAsync(this IAuthService authService, AuthUser? returnedUser)
        {
            authService.LoginUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(returnedUser);
        }

        public static void MockIsEmailConfirmedAsync(this IAuthService authService, bool isConfirmed)
        {
            authService.IsEmailConfirmedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(isConfirmed);
        }

        public static void MockRegisterUserAsync(this IAuthService authService, bool success, string[] errors)
        {
            authService.RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>())
                .Returns((success, errors));
        }

        public static void MockGenerateEmailConfirmationTokenAsync(this IAuthService authService, string tokenResponse)
        {
            authService.GenerateEmailConfirmationTokenAsync(Arg.Any<string>())
                .Returns(tokenResponse);
        }

        // --- TokenService ---
        public static void MockGenerateRefreshTokenAsync(this ITokenService tokenService, RefreshToken mockedToken)
        {
            tokenService.GenerateRefreshTokenAsync(Arg.Any<string>())
                .Returns(mockedToken);
        }

        public static void MockGenerateAccessToken(this ITokenService tokenService, string mockedToken)
        {
            tokenService.GenerateAccessToken(Arg.Any<AuthUser>())
                .Returns(mockedToken);
        }

        // --- PaymentService ---
        public static void MockGetAuthTokenAsync(this IPaymentService paymentService, string token)
        {
            paymentService.GetAuthTokenAsync().Returns(token);
        }

        public static void MockCreateOrderAsync(this IPaymentService paymentService, int orderId)
        {
            paymentService.CreateOrderAsync(Arg.Any<string>(), Arg.Any<decimal>()).Returns(orderId);
        }

        public static void MockGetPaymentKeyAsync(this IPaymentService paymentService, string paymentKey)
        {
            paymentService.GetPaymentKeyAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<decimal>(), Arg.Any<object>())
                .Returns(paymentKey);
        }

        // --- ImageService ---
        public static void MockUploadImageAsync(this IImageService imageService, string returnedUrl)
        {
            imageService.UploadImageAsync(Arg.Any<Stream>(), Arg.Any<string>())
                .Returns(returnedUrl);
        }
    }
}