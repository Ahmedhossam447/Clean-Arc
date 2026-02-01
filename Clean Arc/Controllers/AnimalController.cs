using Clean_Arc.Extensions;
using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clean_Arc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnimalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreateAnimalResponse>> CreateAnimal([FromBody] CreateAnimalCommand command)
    {
        var result = await _mediator.Send(command);
        return result;
    }

    [Authorize]
    [HttpPost("{animalid}/Adopt")]
    public async Task<ActionResult<AdoptAnimalResponse>> AdoptAnimal([FromRoute] int animalid)
    {
        var adopterid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (adopterid == null)
            return Unauthorized();

        var command = new AdoptAnimalCommand
        {
            AnimalId = animalid,
            AdopterId = adopterid
        };

        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }

    // GET

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReadAnimalResponse>> GetAnimalById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var query = new GetAnimalByIdQuery { AnimalId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<ReadAnimalResponse>>> GetAllAnimals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllAnimalsQuery
        {
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return result;
    }

    [HttpGet("Available/{userId:guid}")]
    public async Task<ActionResult<PaginationResponse<ReadAnimalResponse>>> GetAvailableAnimalsForAdoption(
        [FromRoute] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableAnimalsForAdoptionQuery
        {
            UserId = userId,
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return result;
    }

    [HttpGet("Search")]
    public async Task<ActionResult<PaginationResponse<ReadAnimalResponse>>> SearchAnimals(
        [FromQuery] string? type,
        [FromQuery] string? breed,
        [FromQuery] string? gender,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchAnimalsQuery
        {
            Type = type,
            Breed = breed,
            Gender = gender,
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return result;
    }

    [Authorize]
    [HttpGet("Owner/{ownerId}")]
    public async Task<ActionResult<PaginationResponse<ReadAnimalResponse>>> GetAnimalsByOwner(
        [FromRoute] string ownerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnimalsByOwnerQuery
        {
            OwnerId = ownerId,
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return result;
    }

    // PUT

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateAnimalResponse>> UpdateAnimal(int id, [FromBody] UpdateAnimalCommand command)
    {
        command.AnimalId = id;
        var result = await _mediator.Send(command);
        return result;
    }

    // DELETE

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteAnimalResponse>> DeleteAnimal([FromRoute] int id)
    {
        var command = new DeleteAnimalCommand { AnimalId = id };
        var result = await _mediator.Send(command);
        return result.ToActionResult(this);
    }
}
