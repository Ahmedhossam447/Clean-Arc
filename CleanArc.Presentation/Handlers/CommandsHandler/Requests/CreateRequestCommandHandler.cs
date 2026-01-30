using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Result<CreateRequestResponse>>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Animal> _animalRepository;
        private readonly IDistributedCache _cache;

        public CreateRequestCommandHandler(
            IRepository<Request> requestRepository, 
            IRepository<Animal> animalRepository,
            IDistributedCache cache)
        {
            _requestRepository = requestRepository;
            _animalRepository = animalRepository;
            _cache = cache;
        }

        public async Task<Result<CreateRequestResponse>> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(command.AnimalId);
            if (animal == null)
            {
                return Animal.Errors.NotFound;
            }

            if (animal.IsAdopted)
            {
                return Animal.Errors.AlreadyAdopted;
            }

            if (animal.Userid == command.RequesterId)
            {
                return Request.Errors.CannotRequestOwnAnimal;
            }

            var existingRequests = await _requestRepository.GetAsync(
                r => r.AnimalId == command.AnimalId && r.Useridreq == command.RequesterId);
            
            if (existingRequests.Any())
            {
                return Request.Errors.AlreadyExists;
            }

            var request = new Request
            {
                Userid = animal.Userid,
                Useridreq = command.RequesterId,
                AnimalId = command.AnimalId,
                Status = "Pending"
            };

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{command.RequesterId}");

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
