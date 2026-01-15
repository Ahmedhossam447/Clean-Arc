using CleanArc.Application.Contracts.Responses.Request;
using MediatR;

namespace CleanArc.Application.Commands.Request
{
    public class CreateRequestCommand : IRequest<CreateRequestResponse>
    {
        public string Userid { get; set; }
        public string Useridreq { get; set; }
        public int AnimalId { get; set; }
    }
}
