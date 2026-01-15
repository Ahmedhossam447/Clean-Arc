using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, CreateRequestResponse>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Animal> _animalRepository;

        public CreateRequestCommandHandler(IRepository<Request> requestRepository, IRepository<Animal> animalRepository)
        {
            _requestRepository = requestRepository;
            _animalRepository = animalRepository;
        }

        public async Task<CreateRequestResponse> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
        {
            // Verify animal exists and is available
            var animal = await _animalRepository.GetByIdAsync(command.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {command.AnimalId} not found");
            }

            if (animal.IsAdopted)
            {
                throw new InvalidOperationException("This animal has already been adopted");
            }
            if (animal.Userid != command.Useridreq)
            {
                throw new InvalidOperationException("the request has to be to the animal owner");
            }

            var request = new Request
            {
                Userid = command.Userid,
                Useridreq = command.Useridreq,
                AnimalId = command.AnimalId,
                Status = "Pending"
            };
            

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();

            var response = new CreateRequestResponse
            {
                Reqid = request.Reqid,
                Userid = request.Userid,
                Useridreq = request.Useridreq,
                AnimalId = request.AnimalId,
                Status = request.Status
            };

            return response;
        }
    }
}
