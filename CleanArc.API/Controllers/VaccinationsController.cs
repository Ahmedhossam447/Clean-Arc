using CleanArc.API.Extensions;
using CleanArc.Application.Commands.Vaccination;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VaccinationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/vaccinations
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VaccinationResponse>> Add([FromBody] AddVaccinationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        // PUT api/vaccinations/{id}
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<VaccinationResponse>> Update(
            [FromRoute] int id,
            [FromBody] UpdateVaccinationCommand command)
        {
            command.VaccinationId = id;
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }

        // DELETE api/vaccinations/{id}
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var command = new DeleteVaccinationCommand { VaccinationId = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult(this);
        }
    }
}
