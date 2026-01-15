using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, DeleteAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        public DeleteAnimalCommandHandler(IRepository<Animal> animalRepository)
        {
            _animalRepository = animalRepository;
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

            var response = new DeleteAnimalResponse
            {
                AnimalId = request.AnimalId,
                Message = "Animal deleted successfully."
            };
            return response;
        }
    }
}
