using CleanArc.Application.Commands.Chat;
using CleanArc.Application.Queries.Chat;
using CleanArc.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<ActionResult<List<Message>>> GetHistory(
            [FromRoute] string otherUserId,
            [FromQuery] DateTime? before,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetChatHistoryQuery
            {
                UserId = userId,
                OtherUserId = otherUserId,
                BeforeDate = before,
                Limit = Math.Min(limit, 100)
            };

            var messages = await _mediator.Send(query, cancellationToken);
            return Ok(messages);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<List<Message>>> GetUnread(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetUnreadMessagesQuery { UserId = userId };
            var messages = await _mediator.Send(query, cancellationToken);
            return Ok(messages);
        }

        [HttpPut("read/{senderId}")]
        public async Task<ActionResult> MarkAsRead([FromRoute] string senderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new MarkMessagesAsReadCommand
            {
                UserId = userId,
                SenderId = senderId
            };

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
