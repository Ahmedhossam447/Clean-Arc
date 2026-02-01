using Clean_Arc.Extensions;
using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Commands.Token;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Contracts.Responses.Token;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clean_Arc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterCommand register)
        {
            var result = await _mediator.Send(register);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginCommand login)
        {
            var result = await _mediator.Send(login);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh(RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponse>> Logout(LogoutCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }
        [HttpGet("confirm-email")]
        public async Task<ActionResult<ConfirmEmailResponse>> ConfirmEmail([FromQuery] ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPassword(
            [FromQuery] string email, 
            [FromQuery] string token, 
            [FromBody] string newPassword)
        {
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword(ChangePasswordCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }
    }
}
