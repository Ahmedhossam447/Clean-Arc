using CleanArc.Application.Commands.Request;
using CleanArc.Application.Handlers.CommandsHandler.Requests;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace CleanArc.Testing.Unit.RequestTests
{
    public class UpdateRequestHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IRepository<Request> _requestRepository;
        private readonly UpdateRequestCommandHandler _handler;

        public UpdateRequestHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cache = Substitute.For<IDistributedCache>();
            _requestRepository = Substitute.For<IRepository<Request>>();

            _unitOfWork.Repository<Request>().Returns(_requestRepository);

            _handler = new UpdateRequestCommandHandler(_unitOfWork, _cache);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRequestDoesNotExist()
        {
            // Arrange
            var command = new UpdateRequestCommand { RequestId = 1 };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Request.Errors.NotFound);
        }

        [Fact]
        public async Task Handle_Should_UpdateOnlyProvidedFields()
        {
            // Arrange
            var command = new UpdateRequestCommand 
            { 
                RequestId = 1,
                Status = "Updated"
                // OwnerId, RequesterId, AnimalId are all null => should not change
            };
            var request = new Request 
            { 
                Id = 1, 
                OwnerId = "owner1", 
                RequesterId = "user1", 
                AnimalId = 5, 
                Status = "Pending" 
            };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.OwnerId.Should().Be("owner1"); // Unchanged
            result.Value.RequesterId.Should().Be("user1"); // Unchanged
            result.Value.AnimalId.Should().Be(5); // Unchanged
            result.Value.Status.Should().Be("Updated"); // Changed

            _requestRepository.Received(1).Update(Arg.Is<Request>(r => r.Status == "Updated"));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_Should_InvalidateBothCacheKeys_WhenRequesterIdChanges()
        {
            // Arrange
            var command = new UpdateRequestCommand 
            { 
                RequestId = 1,
                RequesterId = "new_user"
            };
            var request = new Request 
            { 
                Id = 1, 
                OwnerId = "owner1", 
                RequesterId = "old_user",
                AnimalId = 5,
                Status = "Pending" 
            };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.RequesterId.Should().Be("new_user");

            // Both old and new requester caches should be invalidated
            await _cache.Received(1).RemoveAsync($"requests:user:old_user", Arg.Any<CancellationToken>());
            await _cache.Received(1).RemoveAsync($"requests:user:new_user", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_InvalidateOnlyOneCacheKey_WhenRequesterIdStaysSame()
        {
            // Arrange
            var command = new UpdateRequestCommand 
            { 
                RequestId = 1,
                Status = "Approved"
            };
            var request = new Request 
            { 
                Id = 1, 
                OwnerId = "owner1", 
                RequesterId = "user1",
                AnimalId = 5,
                Status = "Pending" 
            };

            _requestRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Request?>(request));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Only one cache removal (since requester didn't change)
            await _cache.Received(1).RemoveAsync($"requests:user:user1", Arg.Any<CancellationToken>());
        }
    }
}
