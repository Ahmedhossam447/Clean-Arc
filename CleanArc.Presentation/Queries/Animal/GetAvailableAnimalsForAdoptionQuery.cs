using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAvailableAnimalsForAdoptionQuery :IRequest<GetAvailableAnimalsForAdoptionResponse>
    {
        public string UserId { get; set; }
        }
}
