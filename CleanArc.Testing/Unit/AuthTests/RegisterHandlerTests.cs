using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Handlers.CommandsHandler.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CleanArc.Testing.Unit.AuthTests
{
    public class RegisterHandlerTests
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly RegisterCommandHandler _handler;

        public RegisterHandlerTests()
        {
            _authService = Substitute.For<IAuthService>();
            _emailService = Substitute.For<IEmailService>();
            _configuration = Substitute.For<IConfiguration>();
            
            // Mocking IConfiguration indexing
            _configuration["AppSettings:BaseUrl"].Returns("https://test.local");

            _handler = new RegisterCommandHandler(_authService, _emailService, _configuration);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_AndSendEmail_WhenRegistrationSucceeds()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                Username = "testuser", 
                Password = "Password123!", 
                Email = "test@test.local", 
                Role = "User", 
                FullName = "Test User" 
            };

            _authService.MockRegisterUserAsync(true, Array.Empty<string>());
            _authService.MockGenerateEmailConfirmationTokenAsync("mock-email-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            // Verify email token was generated
            await _authService.Received(1).GenerateEmailConfirmationTokenAsync(command.Email);

            // Verify email service was called to send the confirmation link
            await _emailService.Received(1).SendEmailAsync(
                command.Email,
                Arg.Any<string>(), // Subject
                Arg.Is<string>(body => body.Contains("mock-email-token") && body.Contains("https://test.local")), // Body contains link
                true // isHtml
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRegistrationFails()
        {
            // Arrange
            var command = new RegisterCommand 
            { 
                Username = "existinguser", 
                Email = "test@test.local",
                Password = "Password123!"
            };

            var errors = new string[] { "Username 'existinguser' is already taken." };

            _authService.MockRegisterUserAsync(false, errors);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("User.RegistrationFailed");
            result.Error.Description.Should().Contain("Username 'existinguser' is already taken.");

            // Verify no email token was generated or sent
            await _authService.DidNotReceiveWithAnyArgs().GenerateEmailConfirmationTokenAsync(default);
            await _emailService.DidNotReceiveWithAnyArgs().SendEmailAsync(default, default, default);
        }
    }
}
