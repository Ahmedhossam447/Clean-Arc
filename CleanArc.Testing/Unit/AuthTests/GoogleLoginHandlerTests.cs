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
    public class GoogleLoginHandlerTests
    {
        private readonly ITokenService _tokenService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly GoogleLoginCommandHandler _handler;

        public GoogleLoginHandlerTests()
        {
            _tokenService = Substitute.For<ITokenService>();
            _googleAuthService = Substitute.For<IGoogleAuthService>();
            _userService = Substitute.For<IUserService>();
            _authService = Substitute.For<IAuthService>();
            
            _handler = new GoogleLoginCommandHandler(_tokenService, _googleAuthService, _userService, _authService);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenGoogleTokenIsInvalid()
        {
            // Arrange
            var command = new GoogleLoginCommand { TokenId = "invalid-token" };
            
            _googleAuthService.MockValidateTokenAsync(Result<GoogleUser>.Failure(UserErrors.NotFound));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_LoginExistingUser_WhenEmailAlreadyExists()
        {
            // Arrange
            var command = new GoogleLoginCommand { TokenId = "valid-token" };
            var googleUser = new GoogleUser("existing@google.com", "John Doe", "subj-123");
            var existingDbUser = new AuthUser { Id = "u-1", Email = "existing@google.com", UserName = "johndoe" };
            var mockToken = new RefreshToken { Token = "mock-refresh" };

            _googleAuthService.MockValidateTokenAsync(Result<GoogleUser>.Success(googleUser));
            _userService.MockGetUserByEmailAsync(existingDbUser);
            _authService.MockIsEmailConfirmedAsync(true); // Already confirmed

            _tokenService.MockGenerateAccessToken("mock-access");
            _tokenService.MockGenerateRefreshTokenAsync(mockToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Email.Should().Be("existing@google.com");
            result.Value.AccessToken.Should().Be("mock-access");

            // Verify it added the external login and generated tokens
            await _authService.Received(1).AddExternalLoginAsync(existingDbUser.Email, "Google", googleUser.Subject);
            
            // Verify it didn't try to register
            await _authService.DidNotReceiveWithAnyArgs().RegisterUserAsync(default, default, default, default, default, default);
        }

        [Fact]
        public async Task Handle_Should_RegisterNewUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var command = new GoogleLoginCommand { TokenId = "valid-token-new-user" };
            var googleUser = new GoogleUser("new@google.com", "Jane Doe", "subj-456");
            var newDbUser = new AuthUser { Id = "u-2", Email = "new@google.com", UserName = "new_xxxx" };
            var mockToken = new RefreshToken { Token = "mock-refresh" };

            _googleAuthService.MockValidateTokenAsync(Result<GoogleUser>.Success(googleUser));

            // First call returns null (user not found), second call returns the mocked new user
            _userService.MockGetUserByEmailAsync((AuthUser)null, newDbUser);

            _authService.MockRegisterUserAsync(true, Array.Empty<string>()); // Simulated success from Identity
            _authService.MockIsEmailConfirmedAsync(true); // Assuming the registration forces it to true

            _tokenService.MockGenerateAccessToken("mock-access");
            _tokenService.MockGenerateRefreshTokenAsync(mockToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            // Verify RegisterUserAsync was called exactly once with the correct email
            await _authService.Received(1).RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), googleUser.Email, true, "User", googleUser.Name);
            
            // Verify it mapped the external login
            await _authService.Received(1).AddExternalLoginAsync(newDbUser.Email, "Google", googleUser.Subject);
        }
        
        [Fact]
        public async Task Handle_Should_VerifyEmail_WhenExistingUserEmailIsNotConfirmed()
        {
            // Arrange
            var command = new GoogleLoginCommand { TokenId = "valid-token" };
            var googleUser = new GoogleUser("unconfirmed@google.com", "Unconfirmed User", "subj-unconfirmed");
            var existingDbUser = new AuthUser { Id = "u-unc", Email = "unconfirmed@google.com" };

            _googleAuthService.MockValidateTokenAsync(Result<GoogleUser>.Success(googleUser));
            _userService.MockGetUserByEmailAsync(existingDbUser);
            _authService.MockIsEmailConfirmedAsync(false); // Email NOT confirmed

            _tokenService.MockGenerateAccessToken("mock-access");
            _tokenService.MockGenerateRefreshTokenAsync(new RefreshToken());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            // It should explicitly verify the email since it wasn't confirmed
            await _authService.Received(1).VerifyEmailForGoogleAuthAsync(existingDbUser.Email);
        }
    }
}
