using CleanArc.API.Contracts.Request;
using CleanArc.API.Extensions;
using CleanArc.Application.Commands.Product;
using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Product;
using CleanArc.Application.Queries.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArc.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST

    [Authorize(Roles = "Shelter")]
    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct([FromForm] CreateProductRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ShelterId = userId,
            Image = request.Image?.OpenReadStream(),
            FileName = request.Image?.FileName ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    // GET

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReadProductResponse>> GetProductById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { ProductId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<ReadProductResponse>>> GetAllProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProductsQuery
        {
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return result;
    }

    // PUT

    [Authorize(Roles = "Shelter")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateProductResponse>> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new UpdateProductCommand
        {
            ProductId = id,
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Image = request.Image?.OpenReadStream(),
            FileName = request.Image?.FileName ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    // DELETE

    [Authorize(Roles = "Shelter")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteProductResponse>> DeleteProduct([FromRoute] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new DeleteProductCommand
        {
            ProductId = id,
            UserId = userId
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }
}
