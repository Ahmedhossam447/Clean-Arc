using Clean_Arc.Extensions;
using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Requests.Order;
using CleanArc.Application.Contracts.Responses.Order;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clean_Arc.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] List<CartItemRequest> items)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var email = User.FindFirstValue(ClaimTypes.Email);

        var command = new CreateOrderCommand
        {
            Items = items,
            CustomerId = userId,
            CustomerEmail = email
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }
}
