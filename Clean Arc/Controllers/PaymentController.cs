using CleanArc.Application.Commands.Payment;
using Clean_Arc.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Arc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("initiate")]
        public async Task<ActionResult<object>> Initiate([FromBody] InitiatePaymentCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return result.ToActionResult(this);

            return Ok(new { url = result.Value });
        }
    }
}

