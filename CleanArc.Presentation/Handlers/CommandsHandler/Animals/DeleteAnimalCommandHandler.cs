using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, Result<DeleteAnimalResponse>>
    {
        private readonly IRepository<Animal> _animalRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IDistributedCache _cache;

        public DeleteAnimalCommandHandler(
            IRepository<Animal> animalRepository, 
            IRepository<Request> requestRepository,
            IDistributedCache cache)
        {
            _animalRepository = animalRepository;
            _requestRepository = requestRepository;
            _cache = cache;
        }

        public async Task<Result<DeleteAnimalResponse>> Handle(DeleteAnimalCommand command, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(command.AnimalId, cancellationToken);
            if (animal == null)
            {
                return Animal.Errors.NotFound;
            }

            var requests = await _requestRepository.GetAsync(r => r.AnimalId == command.AnimalId, cancellationToken);
            var affectedUserIds = requests.Select(r => r.Useridreq).Distinct().ToList();

            foreach (var req in requests)
            {
                await _requestRepository.Delete(req.Reqid);
            }

            await _animalRepository.Delete(animal.AnimalId);
            await _animalRepository.SaveChangesAsync();

            // Cache invalidation after write - don't use cancellationToken
            await _cache.RemoveAsync($"animal:{animal.AnimalId}");
            await _cache.RemoveAsync($"animals:available:{animal.Userid}");

            foreach (var userId in affectedUserIds)
            {
                await _cache.RemoveAsync($"requests:user:{userId}");
            }

            return new DeleteAnimalResponse
            {
                AnimalId = command.AnimalId,
                Message = "Animal deleted successfully."
            };
        }
    }
}
