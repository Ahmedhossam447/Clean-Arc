using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Animals;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.AnimalTests
{
    public class DeleteAnimalHandlerTests
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IDistributedCache _Cache;
        private readonly IImageService _ImageService;
        private readonly IBackgroundJobService _BackgroundJobService;
        private readonly ILogger<DeleteAnimalCommandHandler> _Logger;
        private readonly DeleteAnimalCommandHandler _handler;

        public DeleteAnimalHandlerTests()
        {
            _UnitOfWork = Substitute.For<IUnitOfWork>();
            _Cache = Substitute.For<IDistributedCache>();
            _ImageService = Substitute.For<IImageService>();
            _BackgroundJobService = Substitute.For<IBackgroundJobService>();
            _Logger = Substitute.For<ILogger<DeleteAnimalCommandHandler>>();
            _handler = new DeleteAnimalCommandHandler(_UnitOfWork, _Cache, _ImageService, _Logger, _BackgroundJobService);
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_DeleteAnimal_When_OwnerDeletes()
        {
            var animal = new Animal
            {
                AnimalId = 1,
                Name = "Simba",
                Userid = "owner-123",
                Photo = "https://s3.amazonaws.com/bucket/simba.jpg"
            };
            var command = new DeleteAnimalCommand { AnimalId = 1, UserId = "owner-123" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdAsync(animal);
            _UnitOfWork.Repository<Request>().MockGetRequestsByAnimalId(new List<Request>());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.AnimalId.Should().Be(1);
            result.Value.Message.Should().Contain("deleted");

            _BackgroundJobService.Received(1).EnqueueJob<IImageService>(Arg.Any<Expression<Func<IImageService, Task>>>());
            await _UnitOfWork.Repository<Animal>().Received(1).Delete(1);
            await _UnitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_Fail_When_AnimalNotFound()
        {
            var command = new DeleteAnimalCommand { AnimalId = 999, UserId = "owner-123" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdReturnsNull(999);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.NotFound");

            await _UnitOfWork.Repository<Animal>().DidNotReceive().Delete(Arg.Any<int>());
            await _UnitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_Fail_When_UserIsNotOwner()
        {
            var animal = new Animal
            {
                AnimalId = 1,
                Name = "Simba",
                Userid = "owner-123",
                Photo = "https://s3.amazonaws.com/bucket/simba.jpg"
            };
            var command = new DeleteAnimalCommand { AnimalId = 1, UserId = "other-user-456" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdAsync(animal);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.Unauthorized");

            await _UnitOfWork.Repository<Animal>().DidNotReceive().Delete(Arg.Any<int>());
            await _UnitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_DeleteRelatedRequests()
        {
            var animal = new Animal { AnimalId = 1, Name = "Simba", Userid = "owner-123", Photo = "pic.jpg" };
            var requests = new List<Request>
            {
                new Request { Reqid = 10, AnimalId = 1, Useridreq = "adopter-1" },
                new Request { Reqid = 11, AnimalId = 1, Useridreq = "adopter-2" }
            };
            var command = new DeleteAnimalCommand { AnimalId = 1, UserId = "owner-123" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdAsync(animal);
            _UnitOfWork.Repository<Request>().MockGetRequestsByAnimalId(requests);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            // Should delete each request
            await _UnitOfWork.Repository<Request>().Received(1).Delete(10);
            await _UnitOfWork.Repository<Request>().Received(1).Delete(11);
            await _UnitOfWork.Repository<Animal>().Received(1).Delete(1);
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_NotEnqueueJob_When_NoPhoto()
        {
            var animal = new Animal { AnimalId = 1, Name = "Simba", Userid = "owner-123", Photo = null };
            var command = new DeleteAnimalCommand { AnimalId = 1, UserId = "owner-123" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdAsync(animal);
            _UnitOfWork.Repository<Request>().MockGetRequestsByAnimalId(new List<Request>());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _BackgroundJobService.DidNotReceive().EnqueueJob<IImageService>(Arg.Any<Expression<Func<IImageService, Task>>>());
        }

        [Fact]
        public async Task DeleteAnimalHandler_Should_InvalidateCache_ForAnimalAndAffectedUsers()
        {
            var animal = new Animal { AnimalId = 5, Name = "Rex", Userid = "owner-1", Photo = "img.jpg" };
            var requests = new List<Request>
            {
                new Request { Reqid = 1, AnimalId = 5, Useridreq = "user-a" },
                new Request { Reqid = 2, AnimalId = 5, Useridreq = "user-b" },
                new Request { Reqid = 3, AnimalId = 5, Useridreq = "user-a" } // duplicate user
            };
            var command = new DeleteAnimalCommand { AnimalId = 5, UserId = "owner-1" };

            _UnitOfWork.Repository<Animal>().MockGetAnimalByIdAsync(animal);
            _UnitOfWork.Repository<Request>().MockGetRequestsByAnimalId(requests);

            await _handler.Handle(command, CancellationToken.None);

            await _Cache.Received(1).RemoveAsync("animal:5", Arg.Any<CancellationToken>());
            await _Cache.Received(1).RemoveAsync("animals:available:owner-1", Arg.Any<CancellationToken>());
            await _Cache.Received(1).RemoveAsync("requests:user:user-a", Arg.Any<CancellationToken>());
            await _Cache.Received(1).RemoveAsync("requests:user:user-b", Arg.Any<CancellationToken>());
        }
    }
}
