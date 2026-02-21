using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Requests;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace CleanArc.Testing.Unit.RequestTests
{
    public class AcceptRequestHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Request> _requestRepository;
        private readonly AcceptRequestCommandHandler _handler;

        public AcceptRequestHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _mediator = Substitute.For<IMediator>();
            _notificationService = Substitute.For<INotificationService>();
            _requestRepository = Substitute.For<IRepository<Request>>();

            _unitOfWork.Repository<Request>().Returns(_requestRepository);

            _handler = new AcceptRequestCommandHandler(_unitOfWork, _cache, _mediator, _notificationService);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequestDoesNotExist()
        {
            // Arrange
            var command = new AcceptRequestCommand { RequestId = 1, OwnerId = "owner1" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUserIsNotOwner()
        {
            // Arrange
            var command = new AcceptRequestCommand { RequestId = 1, OwnerId = "not_owner" };
            var request = new Request { Id = 1, OwnerId = "real_owner", RequesterId = "user1", Status = "Pending" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequestAlreadyApproved()
        {
            // Arrange
            var command = new AcceptRequestCommand { RequestId = 1, OwnerId = "owner1" };
            var request = new Request { Id = 1, OwnerId = "owner1", RequesterId = "user1", Status = "Approved" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.AlreadyApproved);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenAdoptionFails()
        {
            // Arrange
            var command = new AcceptRequestCommand { RequestId = 1, OwnerId = "owner1" };
            var request = new Request { Id = 1, OwnerId = "owner1", RequesterId = "user1", AnimalId = 5, Status = "Pending" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Simulate adoption failure
            _mediator.Send(Arg.Any<AdoptAnimalCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<AdoptAnimalResponse>.Failure(Animal.Errors.AlreadyAdopted));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Animal.Errors.AlreadyAdopted);
        }

        [Fact]
        public async Task Handle_Should_ApproveRequestAndNotify_WhenAllValidationsPass()
        {
            // Arrange
            var command = new AcceptRequestCommand { RequestId = 1, OwnerId = "owner1" };
            var request = new Request { Id = 1, OwnerId = "owner1", RequesterId = "user1", AnimalId = 5, Status = "Pending" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            _mediator.Send(Arg.Any<AdoptAnimalCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<AdoptAnimalResponse>.Success(new AdoptAnimalResponse()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _requestRepository.Received(1).Update(Arg.Is<Request>(r => r.Status == "Approved"));
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _notificationService.Received(1).SendNotificationToUserAsync(
                "user1", "RequestApproved", Arg.Any<object>());
            await _cache.Received(1).RemoveAsync($"requests:user:user1", Arg.Any<CancellationToken>());
        }
    }
}
