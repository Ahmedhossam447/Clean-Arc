using CleanArc.Application.Commands.Request;
using CleanArc.Application.Handlers.CommandsHandler.Requests;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.RequestTests
{
    public class CreateRequestHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IRepository<Animal> _animalRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly CreateRequestCommandHandler _handler;

        public CreateRequestHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _animalRepository = Substitute.For<IRepository<Animal>>();
            _requestRepository = Substitute.For<IRepository<Request>>();

            _unitOfWork.Repository<Animal>().Returns(_animalRepository);
            _unitOfWork.Repository<Request>().Returns(_requestRepository);

            _handler = new CreateRequestCommandHandler(_unitOfWork, _cache);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenAnimalDoesNotExist()
        {
            // Arrange
            var command = new CreateRequestCommand { AnimalId = 1, RequesterId = "user1" };

            _animalRepository.MockGetAnimalByIdReturnsNull(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Animal.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenAnimalIsAlreadyAdopted()
        {
            // Arrange
            var command = new CreateRequestCommand { AnimalId = 1, RequesterId = "user1" };
            var animal = new Animal { Id = 1, OwnerId = "owner1", IsAdopted = true };

            _animalRepository.MockGetAnimalByIdAsync(animal);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Animal.Errors.AlreadyAdopted);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequesterIsOwner()
        {
            // Arrange
            var command = new CreateRequestCommand { AnimalId = 1, RequesterId = "owner1" };
            var animal = new Animal { Id = 1, OwnerId = "owner1", IsAdopted = false };

            _animalRepository.MockGetAnimalByIdAsync(animal);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.CannotRequestOwnAnimal);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenDuplicateRequestExists()
        {
            // Arrange
            var command = new CreateRequestCommand { AnimalId = 1, RequesterId = "user1" };
            var animal = new Animal { Id = 1, OwnerId = "owner1", IsAdopted = false };
            var existingRequest = new Request { Id = 1, AnimalId = 1, RequesterId = "user1" };

            _animalRepository.MockGetAnimalByIdAsync(animal);
            _requestRepository.MockGetRequestsByAnimalId(new List<Request> { existingRequest });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.AlreadyExists);
        }

        [Fact]
        public async Task Handle_Should_CreateRequest_WhenAllValidationsPass()
        {
            // Arrange
            var command = new CreateRequestCommand { AnimalId = 1, RequesterId = "user1" };
            var animal = new Animal { Id = 1, OwnerId = "owner1", IsAdopted = false };

            _animalRepository.MockGetAnimalByIdAsync(animal);
            _requestRepository.MockGetRequestsByAnimalId(new List<Request>()); // No duplicates

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.RequesterId.Should().Be("user1");
            result.Value.OwnerId.Should().Be("owner1");
            result.Value.AnimalId.Should().Be(1);
            result.Value.Status.Should().Be("Pending");

            await _requestRepository.Received(1).AddAsync(Arg.Is<Request>(r => 
                r.RequesterId == "user1" && r.OwnerId == "owner1" && r.Status == "Pending"));
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _cache.Received(1).RemoveAsync($"requests:user:user1", Arg.Any<CancellationToken>());
        }
    }
}
