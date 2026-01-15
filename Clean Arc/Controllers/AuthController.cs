using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
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
        [HttpPost("Register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterCommand register)
        {
            var result = await _mediator.Send(register);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginCommand login)
        {
            var result = await _mediator.Send(login);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
