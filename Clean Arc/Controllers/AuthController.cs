using Clean_Arc.Extensions;
using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Commands.Token;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Contracts.Responses.Token;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    }
}
