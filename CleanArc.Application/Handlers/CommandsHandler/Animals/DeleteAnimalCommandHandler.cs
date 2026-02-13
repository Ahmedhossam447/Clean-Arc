using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, Result<DeleteAnimalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<DeleteAnimalCommandHandler> _logger;

        public DeleteAnimalCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            IImageService imageService,
            ILogger<DeleteAnimalCommandHandler> logger,
            IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _imageService = imageService;
            _logger = logger;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result<DeleteAnimalResponse>> Handle(DeleteAnimalCommand command, CancellationToken cancellationToken)
        {
            var animalRepo = _unitOfWork.Repository<Animal>();
            var requestRepo = _unitOfWork.Repository<Request>();

            var animal = await animalRepo.GetByIdAsync(command.AnimalId, cancellationToken);
            if (animal == null)
            {
                return Animal.Errors.NotFound;
            }

            if (animal.OwnerId != command.UserId)
            {
                return Animal.Errors.Unauthorized;
            }

            if (!string.IsNullOrEmpty(animal.Photo))
            {
                _backgroundJobService.EnqueueJob<IImageService>(x => x.DeleteImageAsync(animal.Photo));
            }

            var requests = await requestRepo.GetAsync(r => r.AnimalId == command.AnimalId, cancellationToken);
            var affectedUserIds = requests.Select(r => r.RequesterId).Distinct().ToList();

            foreach (var req in requests)
            {
                await requestRepo.Delete(req.Id);
            }

            await animalRepo.Delete(animal.Id);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync($"animal:{animal.Id}");
            await _cache.RemoveAsync($"animals:available:{animal.OwnerId}");

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
