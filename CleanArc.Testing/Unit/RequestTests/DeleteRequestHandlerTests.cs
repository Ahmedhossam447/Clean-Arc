using CleanArc.Application.Commands.Request;
using CleanArc.Application.Handlers.CommandsHandler.Requests;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace CleanArc.Testing.Unit.RequestTests
{
    public class DeleteRequestHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IRepository<Request> _requestRepository;
        private readonly DeleteRequestCommandHandler _handler;

        public DeleteRequestHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _requestRepository = Substitute.For<IRepository<Request>>();

            _unitOfWork.Repository<Request>().Returns(_requestRepository);

            _handler = new DeleteRequestCommandHandler(_unitOfWork, _cache);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequestDoesNotExist()
        {
            // Arrange
            var command = new DeleteRequestCommand { RequestId = 1, UserId = "user1" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUserIsNotRequester()
        {
            // Arrange
            var command = new DeleteRequestCommand { RequestId = 1, UserId = "different_user" };
            var request = new Request { Id = 1, RequesterId = "original_user" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_DeleteRequestSuccessfully()
        {
            // Arrange
            var command = new DeleteRequestCommand { RequestId = 1, UserId = "user1" };
            var request = new Request { Id = 1, RequesterId = "user1" };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            await _requestRepository.Received(1).Delete(command.RequestId);
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _cache.Received(1).RemoveAsync($"requests:user:user1", Arg.Any<CancellationToken>());
        }
    }
}
