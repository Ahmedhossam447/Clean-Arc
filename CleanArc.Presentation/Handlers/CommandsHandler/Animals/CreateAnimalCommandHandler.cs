using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand, CreateAnimalResponse>
    {
        private readonly IRepository<Core.Entites.Animal> _animalRepository;
        private readonly IDistributedCache _cache;

        public CreateAnimalCommandHandler(
            IRepository<Core.Entites.Animal> animalRepository,
            IDistributedCache cache)
        {
            _animalRepository = animalRepository;
            _cache = cache;
        }

        public async Task<CreateAnimalResponse> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            // Create Animal with MedicalRecord navigation property
            // EF Core will automatically create MedicalRecord in the same transaction
            var animal = new Core.Entites.Animal
            {
                Name = request.Name,
                Age = request.Age,
                Type = request.Type,
                Breed = request.Breed,
                About = request.About,
                IsAdopted = false,
                Userid = request.Userid,
                Gender = request.Gender,
                Photo = request.Photo,
                // EF Core will automatically create MedicalRecord when Animal is saved
                MedicalRecord = new Core.Entites.MedicalRecord
                {
                    Weight = request.Weight,
                    Height = request.Height,
                    BloodType = request.BloodType ?? string.Empty,
                    MedicalHistoryNotes = request.MedicalHistoryNotes ?? string.Empty,
                    Injuries = request.Injuries,
                    Status = request.Status
                }
            };
            
            var animalResponse = await _animalRepository.AddAsync(animal);
            // Single SaveChangesAsync() - both Animal and MedicalRecord saved atomically
            await _animalRepository.SaveChangesAsync();

            // Invalidate the creator's available animals cache (after write - don't use cancellationToken)
            // Other users' caches will expire naturally (2 min TTL)
            await _cache.RemoveAsync($"animals:available:{request.Userid}");

            var response = new CreateAnimalResponse
            {
                AnimalId = animalResponse.AnimalId,
                Name = animalResponse.Name,
                Age = animalResponse.Age,
                Type = animalResponse.Type,
                Breed = animalResponse.Breed,
                About = animalResponse.About,
                Userid = animalResponse.Userid,
                Gender =animalResponse.Gender,
                Photo = animalResponse.Photo
            };
            return response;

        }
    }
}
