using CleanArc.Application.Commands.Request;
using CleanArc.Application.Handlers.CommandsHandler.Requests;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace CleanArc.Testing.Unit.RequestTests
{
    public class RejectRequestHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Request> _requestRepository;
        private readonly RejectRequestCommandHandler _handler;

        public RejectRequestHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _notificationService = Substitute.For<INotificationService>();
            _requestRepository = Substitute.For<IRepository<Request>>();

            _unitOfWork.Repository<Request>().Returns(_requestRepository);

            _handler = new RejectRequestCommandHandler(_unitOfWork, _cache, _notificationService);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequestDoesNotExist()
        {
            // Arrange
            var command = new RejectRequestCommand { RequestId = 1, OwnerId = "owner1" };

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
            var command = new RejectRequestCommand { RequestId = 1, OwnerId = "not_owner" };
            var request = new Request { Id = 1, OwnerId = "real_owner", RequesterId = "user1" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_DeleteRequestAndNotify_WhenSuccessful()
        {
            // Arrange
            var command = new RejectRequestCommand { RequestId = 1, OwnerId = "owner1" };
            var request = new Request { Id = 1, OwnerId = "owner1", RequesterId = "user1", AnimalId = 5 };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            await _requestRepository.Received(1).Delete(request.Id);
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _notificationService.Received(1).SendNotificationToUserAsync(
                "user1", "RequestRejected", Arg.Any<object>());
            await _cache.Received(1).RemoveAsync($"requests:user:user1", Arg.Any<CancellationToken>());
        }
    }
}
