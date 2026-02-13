using CleanArc.API.Extensions;
using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Requests.Order;
using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Application.Queries.Order;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArc.API.Controllers;

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

    [Authorize(Roles = "User")]
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

    [Authorize(Roles = "Shelter")]
    [HttpGet("my-sales")]
    public async Task<ActionResult<PaginationResponse<ShelterSaleResponse>>> GetMySales(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var shelterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(shelterId))
            return Unauthorized();

        var query = new GetShelterSalesQuery
        {
            ShelterId = shelterId,
            PageNumber = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{orderId}/items")]
    public async Task<ActionResult<OrderItemResponse>> AddOrderItem(
        [FromRoute] int orderId,
        [FromBody] CartItemRequest item)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new AddOrderItemCommand
        {
            OrderId = orderId,
            UserId = userId,
            ProductId = item.ProductId,
            Quantity = item.Quantity
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "User")]
    [HttpDelete("{orderId}/items/{itemId}")]
    public async Task<ActionResult> RemoveOrderItem(
        [FromRoute] int orderId,
        [FromRoute] int itemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new RemoveOrderItemCommand
        {
            OrderId = orderId,
            ItemId = itemId,
            UserId = userId
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{orderId}/checkout")]
    public async Task<ActionResult<CheckoutOrderResponse>> Checkout([FromRoute] int orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var email = User.FindFirstValue(ClaimTypes.Email);

        var command = new CheckoutOrderCommand
        {
            OrderId = orderId,
            UserId = userId,
            UserEmail = email
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "Shelter")]
    [HttpPatch("{orderId}/items/{itemId}/status")]
    public async Task<ActionResult> UpdateOrderItemStatus(
        [FromRoute] int orderId,
        [FromRoute] int itemId,
        [FromBody] UpdateOrderItemStatusCommand command)
    {
        var shelterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(shelterId))
            return Unauthorized();

        command.OrderId = orderId;
        command.ItemId = itemId;
        command.ShelterId = shelterId;

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }
}
