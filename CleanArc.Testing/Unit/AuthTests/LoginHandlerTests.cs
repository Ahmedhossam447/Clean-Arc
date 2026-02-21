using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Handlers.CommandsHandler.Auth;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Core.Primitives;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using NSubstitute;

namespace CleanArc.Testing.Unit.AuthTests
{
    public class LoginHandlerTests
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly LoginCommandHandler _handler;

        public LoginHandlerTests()
        {
            _authService = Substitute.For<IAuthService>();
            _tokenService = Substitute.For<ITokenService>();
            _handler = new LoginCommandHandler(_authService, _tokenService);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenCredentialsAreValidAndEmailIsConfirmed()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "Password123" };
            var mockUser = new AuthUser { Id = "u-123", Email = "test@test.com" };
            var mockToken = new RefreshToken { Token = "mock-refresh", ExpiresAt = DateTime.UtcNow.AddDays(7) };

            _authService.MockLoginUserAsync(mockUser);
            _authService.MockIsEmailConfirmedAsync(true);
            _tokenService.MockGenerateRefreshTokenAsync(mockToken);
            _tokenService.MockGenerateAccessToken("mock-access-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.AccessToken.Should().Be("mock-access-token");
            result.Value.RefreshToken.Should().Be("mock-refresh");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenCredentialsAreInvalid()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@test.com", Password = "WrongPassword" };

            _authService.MockLoginUserAsync(null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.InvalidCredentials);
            
            // Ensure token generation wasn't called
            await _tokenService.DidNotReceiveWithAnyArgs().GenerateRefreshTokenAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenEmailIsNotConfirmed()
        {
            // Arrange
            var command = new LoginCommand { Email = "unconfirmed@test.com", Password = "Password123" };
            var mockUser = new AuthUser { Id = "u-456", Email = command.Email };

            _authService.MockLoginUserAsync(mockUser);
            _authService.MockIsEmailConfirmedAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(EmailErrors.NotConfirmed);

            // Ensure token generation wasn't called
            await _tokenService.DidNotReceiveWithAnyArgs().GenerateRefreshTokenAsync(Arg.Any<string>());
        }
    }
}
