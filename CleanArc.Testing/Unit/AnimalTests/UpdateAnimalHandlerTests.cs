using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Animals;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.AnimalTests
{
    public class UpdateAnimalHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<UpdateAnimalCommandHandler> _logger;
        private readonly IRepository<Animal> _animalRepository;
        private readonly UpdateAnimalCommandHandler _handler;

        public UpdateAnimalHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _imageService = Substitute.For<IImageService>();
            _backgroundJobService = Substitute.For<IBackgroundJobService>();
            _logger = Substitute.For<ILogger<UpdateAnimalCommandHandler>>();
            _animalRepository = Substitute.For<IRepository<Animal>>();

            _unitOfWork.Repository<Animal>().Returns(_animalRepository);

            _handler = new UpdateAnimalCommandHandler(
                _unitOfWork, 
                _cache, 
                _imageService, 
                _logger, 
                _backgroundJobService);
        }

        [Fact]
        public async Task Handle_Should_ThrowKeyNotFoundException_WhenAnimalDoesNotExist()
        {
            // Arrange
            var command = new UpdateAnimalCommand { AnimalId = 1, UserId = "user1" };

            _animalRepository.MockGetAnimalByIdReturnsNull(1);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Animal with ID {command.AnimalId} not found");
        }

        [Fact]
        public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotOwner()
        {
            // Arrange
            var command = new UpdateAnimalCommand { AnimalId = 1, UserId = "user2" };
            var animal = new Animal { Id = 1, OwnerId = "user1" };

            _animalRepository.MockGetAnimalByIdAsync(animal);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You are not authorized to update this animal.");
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenPhotoUploadFails()
        {
            // Arrange
            var command = new UpdateAnimalCommand 
            { 
                AnimalId = 1, 
                UserId = "user1",
                Photo = new MemoryStream(),
                fileName = "new_photo.jpg"
            };
            var animal = new Animal { Id = 1, OwnerId = "user1" };

            _animalRepository.MockGetAnimalByIdAsync(animal);
            _imageService.MockUploadImageAsync(""); // Simulate failure

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Failed to upload new photo");
            
            _animalRepository.DidNotReceiveWithAnyArgs().Update(default!);
        }

        [Fact]
        public async Task Handle_Should_UpdateAnimalSuccessfully_WhenNoPhotoIsProvided()
        {
            // Arrange
            var command = new UpdateAnimalCommand 
            { 
                AnimalId = 1, 
                UserId = "user1",
                Name = "Updated Name",
                Age = 5
            };
            var animal = new Animal { Id = 1, OwnerId = "user1", Name = "Old Name", Age = 3 };

            _animalRepository.MockGetAnimalByIdAsync(animal);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Name.Should().Be("Updated Name");
            result.Age.Should().Be(5);
            
            _animalRepository.Received(1).Update(Arg.Is<Animal>(a => a.Name == "Updated Name" && a.Age == 5));
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _cache.Received(1).RemoveAsync($"animal:{animal.Id}", Arg.Any<CancellationToken>());
            await _cache.Received(1).RemoveAsync($"animals:available:{animal.OwnerId}", Arg.Any<CancellationToken>());
            
            _backgroundJobService.DidNotReceiveWithAnyArgs().EnqueueJob<IImageService>(default!);
        }

        [Fact]
        public async Task Handle_Should_UpdateAnimalAndEnqueueOldPhotoDeletion_WhenNewPhotoUploaded()
        {
            // Arrange
            var command = new UpdateAnimalCommand 
            { 
                AnimalId = 1, 
                UserId = "user1",
                Photo = new MemoryStream(),
                fileName = "new_photo.png"
            };
            var animal = new Animal 
            { 
                Id = 1, 
                OwnerId = "user1", 
                Photo = "old_photo_url.png" 
            };

            _animalRepository.MockGetAnimalByIdAsync(animal);
            _imageService.MockUploadImageAsync("new_photo_url.png");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Photo.Should().Be("new_photo_url.png");
            
            _animalRepository.Received(1).Update(Arg.Is<Animal>(a => a.Photo == "new_photo_url.png"));
            await _unitOfWork.Received(1).SaveChangesAsync();
            
            // Verify background job was enqueued for the old photo
            _backgroundJobService.Received(1).EnqueueJob<IImageService>(Arg.Any<Expression<Func<IImageService, Task>>>());
            
            await _cache.Received(1).RemoveAsync($"animal:{animal.Id}", Arg.Any<CancellationToken>());
        }
    }
}
