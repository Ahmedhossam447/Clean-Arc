using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand, Result<CreateAnimalResponse>>
    {
        private readonly IRepository<Core.Entites.Animal> _animalRepository;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly ILogger<CreateAnimalCommandHandler> _logger;

        public CreateAnimalCommandHandler(
            IRepository<Core.Entites.Animal> animalRepository,
            IDistributedCache cache,
            IImageService imageService,
            ILogger<CreateAnimalCommandHandler> logger)
        {
            _animalRepository = animalRepository;
            _cache = cache;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<CreateAnimalResponse>> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            
            if (request.Photo != null)
            {
                // Let exception bubble up to GlobalExceptionHandler if upload fails
                // The handler will catch it and return appropriate error response
                imageUrl = await _imageService.UploadImageAsync(request.Photo, request.fileName);
                
                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("Photo upload returned empty URL for file: {FileName}", request.fileName);
                    return Core.Entites.Animal.Errors.PhotoUploadFailed;
                }
            }

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
                Photo = imageUrl,
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
            try
            {
                await _cache.RemoveAsync($"animals:available:{request.Userid}");
            }
            catch
            {

            }

            var response = new CreateAnimalResponse
            {
                AnimalId = animalResponse.AnimalId,
                Name = animalResponse.Name,
                Age = animalResponse.Age,
                Type = animalResponse.Type,
                Breed = animalResponse.Breed,
                About = animalResponse.About,
                Userid = animalResponse.Userid,
                Gender = animalResponse.Gender,
                Photo = animalResponse.Photo
            };
            
            return response;
        }
    }
}
