using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Queries
{
    public class GetAvailableAnimalsForAdoptionQuery :IRequest<GetAvailableAnimalsForAdoptionResponse>
    {
        public string UserId { get; set; }
        }
}
