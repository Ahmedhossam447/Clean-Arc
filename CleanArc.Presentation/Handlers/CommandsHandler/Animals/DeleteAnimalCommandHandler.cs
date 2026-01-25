using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, DeleteAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        private readonly IDistributedCache _cache;

        public DeleteAnimalCommandHandler(IRepository<Animal> animalRepository, IDistributedCache cache)
        {
            _animalRepository = animalRepository;
            _cache = cache;
        }

        public async Task<DeleteAnimalResponse> Handle(DeleteAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found");
            }

            await _animalRepository.Delete(animal.AnimalId);
            await _animalRepository.SaveChangesAsync();

            // Invalidate the specific animal cache
            await _cache.RemoveAsync($"animal:{animal.AnimalId}", cancellationToken);
            // Invalidate the owner's available animals cache
            await _cache.RemoveAsync($"animals:available:{animal.Userid}", cancellationToken);

            var response = new DeleteAnimalResponse
            {
                AnimalId = request.AnimalId,
                Message = "Animal deleted successfully."
            };
            return response;
        }
    }
}
