using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Vaccination
{
    public class DeleteVaccinationCommand : IRequest<Result>
    {
        [JsonIgnore]
        public int VaccinationId { get; set; }
    }
}
