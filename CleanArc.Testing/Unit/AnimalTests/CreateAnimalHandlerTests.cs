using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Animals;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CleanArc.Testing.Unit.AnimalTests
{
    public class CreateAnimalHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly ILogger<CreateAnimalCommandHandler> _logger;
        private readonly IRepository<Animal> _animalRepository;
        private readonly CreateAnimalCommandHandler _handler;

        public CreateAnimalHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _imageService = Substitute.For<IImageService>();
            _logger = Substitute.For<ILogger<CreateAnimalCommandHandler>>();
            _animalRepository = Substitute.For<IRepository<Animal>>();

            _unitOfWork.Repository<Animal>().Returns(_animalRepository);

            _handler = new CreateAnimalCommandHandler(_unitOfWork, _cache, _imageService, _logger);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenPhotoUploadFails()
        {
            // Arrange
            var command = new CreateAnimalCommand 
            { 
                Name = "Fluffy", 
                OwnerId = "user1",
                Photo = new MemoryStream(),
                fileName = "cat.jpg"
            };

            _imageService.MockUploadImageAsync(""); // Simulate failure (empty string)

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Animal.Errors.PhotoUploadFailed);
            
            await _animalRepository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        }

        [Fact]
        public async Task Handle_Should_CreateAnimal_WhenNoPhotoIsProvided()
        {
            // Arrange
            var command = new CreateAnimalCommand 
            { 
                Name = "Buddy", 
                Age = 2,
                Type = "Dog",
                Breed = "Golden Retriever",
                OwnerId = "user1",
                Photo = null, // No photo
                Weight = 20,
                Status = "Healthy"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Buddy");
            result.Value.Photo.Should().BeNull();
            
            await _animalRepository.Received(1).AddAsync(Arg.Is<Animal>(a => a.Name == "Buddy" && string.IsNullOrEmpty(a.Photo)));
            await _unitOfWork.Received(1).SaveChangesAsync();
            
            // Cache removal should be called using an exact argument string or Arg.Any<string>() if we don't mock the distributed cache extensions exactly
            await _cache.Received(1).RemoveAsync($"animals:available:user1", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_CreateAnimal_WhenPhotoIsUploadedSuccessfully()
        {
            // Arrange
            var command = new CreateAnimalCommand 
            { 
                Name = "Mittens", 
                Type = "Cat",
                OwnerId = "user2",
                Photo = new MemoryStream(),
                fileName = "mittens.png"
            };

            _imageService.MockUploadImageAsync("https://s3.bucket/mittens.png");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Mittens");
            result.Value.Photo.Should().Be("https://s3.bucket/mittens.png");
            
            await _animalRepository.Received(1).AddAsync(Arg.Is<Animal>(a => a.Photo == "https://s3.bucket/mittens.png"));
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _cache.Received(1).RemoveAsync($"animals:available:user2", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_NotFail_WhenCacheRemovalThrows()
        {
            // Arrange
            var command = new CreateAnimalCommand 
            { 
                Name = "Rex", 
                OwnerId = "user1"
            };

            // Simulate cache throwing an exception (e.g. Redis is down)
            _cache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                  .Throws(new Exception("Redis connection failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue(); // Should still succeed (catch block swallows exception)
            await _animalRepository.Received(1).AddAsync(Arg.Any<Animal>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}
