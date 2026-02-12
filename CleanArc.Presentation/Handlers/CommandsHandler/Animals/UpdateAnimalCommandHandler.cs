using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class UpdateAnimalCommandHandler : IRequestHandler<UpdateAnimalCommand, UpdateAnimalResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<UpdateAnimalCommandHandler> _logger;

        public UpdateAnimalCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            IImageService imageService,
            ILogger<UpdateAnimalCommandHandler> logger,
            IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _imageService = imageService;
            _logger = logger;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<UpdateAnimalResponse> Handle(UpdateAnimalCommand request, CancellationToken cancellationToken)
        {
            var animalRepo = _unitOfWork.Repository<Animal>();
            var animal = await animalRepo.GetByIdAsync(request.AnimalId, cancellationToken);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found");
            }

            if (animal.Userid != request.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this animal.");
            }

            string? oldUrl = null;

            if (request.Photo != null && !string.IsNullOrEmpty(request.fileName))
            {
                var newPhotoUrl = await _imageService.UploadImageAsync(request.Photo, request.fileName);
                if (string.IsNullOrEmpty(newPhotoUrl))
                {
                    _logger.LogWarning("Photo upload returned empty URL for animal {AnimalId}", request.AnimalId);
                    throw new Exception("Failed to upload new photo");
                }
                oldUrl = animal.Photo;
                animal.Photo = newPhotoUrl;
            }

            animal.Name = request.Name ?? animal.Name;
            animal.Age = request.Age ?? animal.Age;
            animal.Type = request.Type ?? animal.Type;
            animal.Breed = request.Breed ?? animal.Breed;
            animal.About = request.About ?? animal.About;
            animalRepo.Update(animal);
            await _unitOfWork.SaveChangesAsync();
            if (!string.IsNullOrEmpty(oldUrl))
            {
                _backgroundJobService.EnqueueJob<IImageService>(x => x.DeleteImageAsync(oldUrl));
            }

            await _cache.RemoveAsync($"animal:{animal.AnimalId}");
            await _cache.RemoveAsync($"animals:available:{animal.Userid}");

            var response = new UpdateAnimalResponse
            {
                AnimalId = animal.AnimalId,
                Name = animal.Name,
                Age = animal.Age,
                Type = animal.Type,
                Breed = animal.Breed,
                About = animal.About,
                Userid = animal.Userid,
                Gender = animal.Gender,
                Photo = animal.Photo
            };
            return response;
        }
    }
}
