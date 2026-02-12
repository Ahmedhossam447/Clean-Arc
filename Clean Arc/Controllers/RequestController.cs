using Clean_Arc.Extensions;
using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clean_Arc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "User")]
        [HttpPost("{animalId}")]
        public async Task<ActionResult<CreateRequestResponse>> CreateRequest([FromRoute] int animalId)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            var command = new CreateRequestCommand
            {
                AnimalId = animalId,
                RequesterId = requesterId
            };

            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestResponse>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var query = new GetRequestByIdQuery { RequestId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpGet("my")]
        public async Task<ActionResult<List<RequestResponse>>> GetMyRequests(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetMyRequestsQuery { UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "User")]
        [HttpGet("received")]
        public async Task<ActionResult<List<RequestResponse>>> GetReceivedRequests(CancellationToken cancellationToken)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized();

            var query = new GetReceivedRequestsQuery { OwnerId = ownerId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RequestResponse>> UpdateRequest(
            [FromRoute] int id,
            [FromBody] UpdateRequestCommand command)
        {
            command.RequestId = id;
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpPost("{id}/accept")]
        public async Task<ActionResult> AcceptRequest([FromRoute] int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized();

            var command = new AcceptRequestCommand
            {
                RequestId = id,
                OwnerId = ownerId
            };

            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpPost("{id}/reject")]
        public async Task<ActionResult> RejectRequest([FromRoute] int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized();

            var command = new RejectRequestCommand
            {
                RequestId = id,
                OwnerId = ownerId
            };

            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new DeleteRequestCommand
            {
                RequestId = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }
    }
}
