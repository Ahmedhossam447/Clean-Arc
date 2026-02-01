using Clean_Arc.Extensions;
using CleanArc.Application.Commands.MedicalRecord;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Application.Queries.MedicalRecord;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Arc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET
        [HttpGet("animal/{animalId:int}")]
        public async Task<ActionResult<MedicalRecordResponse>> GetMedicalRecordByAnimalId(
            [FromRoute] int animalId, 
            CancellationToken cancellationToken)
        {
            var query = new GetMedicalRecordByAnimalIdQuery { AnimalId = animalId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult(this);
        }

        // PUT
        [Authorize]
        [HttpPut("animal/{animalId:int}")]
        public async Task<ActionResult<MedicalRecordResponse>> UpdateMedicalRecord(
            [FromRoute] int animalId,
            [FromBody] UpdateMedicalRecordCommand command)
        {
            command.AnimalId = animalId;
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
