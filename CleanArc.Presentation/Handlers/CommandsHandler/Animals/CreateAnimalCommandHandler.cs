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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly ILogger<CreateAnimalCommandHandler> _logger;

        public CreateAnimalCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            IImageService imageService,
            ILogger<CreateAnimalCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Result<CreateAnimalResponse>> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            
            if (request.Photo != null)
            {
                imageUrl = await _imageService.UploadImageAsync(request.Photo, request.fileName);
                
                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("Photo upload returned empty URL for file: {FileName}", request.fileName);
                    return Animal.Errors.PhotoUploadFailed;
                }
            }

            var animal = new Animal
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
            
            await _unitOfWork.Repository<Animal>().AddAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _cache.RemoveAsync($"animals:available:{request.Userid}");
            }
            catch
            {
            }

            var response = new CreateAnimalResponse
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
