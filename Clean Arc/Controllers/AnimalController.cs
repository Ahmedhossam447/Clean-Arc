using CleanArc.Application.Commands;
using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries;
using CleanArc.Application.Queries.Animal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Arc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AnimalController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<CreateAnimalResponse>> CreateAnimal([FromBody] CreateAnimalCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateAnimalResponse>> UpdateAnimal(int id, [FromBody] UpdateAnimalCommand command)
        {
            command.AnimalId = id;
            var result = await _mediator.Send(command);
            return result;
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteAnimalResponse>> DeleteAnimal([FromRoute] int id)
        {
            var command = new DeleteAnimalCommand { AnimalId = id };
            var result = await _mediator.Send(command);
            return result;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadAnimalResponse>> GetAnimalById([FromRoute] int id)
        {
            var query = new GetAnimalByIdQuery { AnimalId = id };
            var result = await _mediator.Send(query);
            return result;
        }
        [HttpGet("Available/{userId:guid}")]
        public async Task<ActionResult<GetAvailableAnimalsForAdoptionResponse>> GetAvailableAnimalsForAdoption([FromRoute] string userId)
        {
            var query = new GetAvailableAnimalsForAdoptionQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result;
        }
        [HttpGet]
        public async Task<ActionResult<GetAllAnimalsResponse>> GetAllAnimals()
        {
            var query = new GetAllAnimalsQuery();
            var result = await _mediator.Send(query);
            return result;
        }

    }
}
