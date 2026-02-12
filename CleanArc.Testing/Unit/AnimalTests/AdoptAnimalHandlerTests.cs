using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Animals;
using CleanArc.Core.Entites;
using CleanArc.Core.Entities;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq.Expressions;

namespace CleanArc.Testing.Unit.AnimalTests
{
    public class AdoptAnimalHandlerTests
    {
        private readonly IUnitOfWork _Repo;
        private readonly IPublishEndpoint _PublishEndpoint;
        private readonly IBackgroundJobService _BackgroundJobService;
        private readonly IUserService _UserService;
        private readonly INotificationService _NotificationService;
        private readonly IDistributedCache _Cache;
        private readonly ILogger<AdoptAnimalCommandHandler> _logger;
        private readonly AdoptAnimalCommandHandler _handler;

        public AdoptAnimalHandlerTests()
        {
            _Repo = Substitute.For<IUnitOfWork>();
            _NotificationService = Substitute.For<INotificationService>();
            _PublishEndpoint = Substitute.For<IPublishEndpoint>();
            _BackgroundJobService = Substitute.For<IBackgroundJobService>();
            _UserService = Substitute.For<IUserService>();
            _Cache = Substitute.For<IDistributedCache>();
            _logger = Substitute.For<ILogger<AdoptAnimalCommandHandler>>();
            _handler = new AdoptAnimalCommandHandler(_Repo, _PublishEndpoint, _UserService, _Cache, _BackgroundJobService, _NotificationService, _logger);
        }

        [Fact]
        public async Task AdoptAnimalHandler_should_adopt_whenAvaliable()
        {
            var command = new AdoptAnimalCommand { AdopterId = "a-123", AnimalId = 1 };
            var animal = new Animal { AnimalId = command.AnimalId, IsAdopted = false, Name = "simba", Userid = "o-456" };
            var request = new Request { Reqid = 10, AnimalId = 1, Useridreq = "a-123" };

            _Repo.AnimalRepository.GetByIdAsync(command.AnimalId, Arg.Any<CancellationToken>()).Returns(animal);
            _UserService.GetUserByIdAsync("o-456", Arg.Any<CancellationToken>())
                .Returns(new CleanArc.Core.Models.Identity.AuthUser { Id = "o-456", UserName = "mohamed", Email = "mohamed@test.com" });
            _UserService.GetUserByIdAsync("a-123", Arg.Any<CancellationToken>())
                .Returns(new CleanArc.Core.Models.Identity.AuthUser { Id = "a-123", UserName = "ahmed", Email = "ahmed@test.com" });
            _Repo.RequestRepository.GetAsync(Arg.Any<Expression<Func<Request, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Request> { request });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.animalId.Should().Be(command.AnimalId);
            animal.IsAdopted.Should().BeTrue();
        }

        [Fact]
        public async Task AdoptAnimalHandler_Should_ThrowException_When_AnimalNotFound()
        {
            var command = new AdoptAnimalCommand { AnimalId = 1, AdopterId = "a-123" };

            _Repo.AnimalRepository.GetByIdAsync(command.AnimalId, Arg.Any<CancellationToken>()).Returns((Animal?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.NotFound");
            await _Repo.DidNotReceive().SaveChangesAsync();
            await _PublishEndpoint.DidNotReceive().Publish(Arg.Any<AnimalAdoptedEvent>(), Arg.Any<CancellationToken>());
            await _UserService.DidNotReceive().GetUserByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AdoptAnimalHandler_Should_Fail_When_AnimalAlreadyAdopted()
        {
            var command = new AdoptAnimalCommand { AdopterId = "a-123", AnimalId = 1 };
            var animal = new Animal { AnimalId = 1, IsAdopted = true, Name = "simba", Userid = "o-456" };
            var request = new Request { Reqid = 10, AnimalId = 1, Useridreq = "a-123" };

            _Repo.AnimalRepository.GetByIdAsync(command.AnimalId, Arg.Any<CancellationToken>()).Returns(animal);
            _UserService.GetUserByIdAsync("o-456", Arg.Any<CancellationToken>())
                .Returns(new CleanArc.Core.Models.Identity.AuthUser { Id = "o-456", UserName = "mohamed", Email = "mohamed@test.com" });
            _UserService.GetUserByIdAsync("a-123", Arg.Any<CancellationToken>())
                .Returns(new CleanArc.Core.Models.Identity.AuthUser { Id = "a-123", UserName = "ahmed", Email = "ahmed@test.com" });
            _Repo.RequestRepository.GetAsync(Arg.Any<Expression<Func<Request, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Request> { request });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.AlreadyAdopted");
        }

        [Fact]
        public async Task AdoptAnimalHandler_Should_Fail_When_OwnerTriesToAdoptOwnAnimal()
        {
            var command = new AdoptAnimalCommand { AdopterId = "o-456", AnimalId = 1 };
            var animal = new Animal { AnimalId = 1, IsAdopted = false, Name = "simba", Userid = "o-456" };
            var request = new Request { Reqid = 10, AnimalId = 1, Useridreq = "o-456" };

            _Repo.AnimalRepository.GetByIdAsync(command.AnimalId, Arg.Any<CancellationToken>()).Returns(animal);
            _UserService.GetUserByIdAsync("o-456", Arg.Any<CancellationToken>())
                .Returns(new CleanArc.Core.Models.Identity.AuthUser { Id = "o-456", UserName = "mohamed", Email = "mohamed@test.com" });
            _Repo.RequestRepository.GetAsync(Arg.Any<Expression<Func<Request, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Request> { request });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.CannotAdoptOwnAnimal");
        }
    }
}
