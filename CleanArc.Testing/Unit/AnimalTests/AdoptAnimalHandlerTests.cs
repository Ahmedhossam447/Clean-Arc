using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Handlers.CommandsHandler.Animals;
using CleanArc.Core.Entites;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Infrastructure.Identity;
using CleanArc.Testing.Unit.Extensions;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace CleanArc.Testing.Unit.AnimalTests
{
    public class AdoptAnimalHandlerTests
    {
        private readonly IRepository<Animal> _Repo;
        private readonly IPublishEndpoint _PublishEndpoint;
        private readonly IUserService _UserService;
        private readonly IDistributedCache _Cache;
        private readonly AdoptAnimalCommandHandler _handler;

        public AdoptAnimalHandlerTests()
        {
            _Repo = Substitute.For<IRepository<Animal>>();
            _PublishEndpoint = Substitute.For<IPublishEndpoint>();
            _UserService = Substitute.For<IUserService>();
            _Cache = Substitute.For<IDistributedCache>();
            _handler = new AdoptAnimalCommandHandler(_Repo, _PublishEndpoint, _UserService, _Cache);
        }
        [Fact]
        public async Task AdoptAnimalHandler_should_adopt_whenAvaliable()
        {
            var command = new AdoptAnimalCommand { AdopterId = "a-123", AnimalId = 1 };
            var animal = new Animal { AnimalId = command.AnimalId, IsAdopted = false, Name = "simba", Userid = "o-456" };
            _Repo.MockGetAnimalByIdAsync(animal);
            _UserService.MockGetUserByIdAsync(command.AdopterId, "ahmed");
            _UserService.MockGetUserByIdAsync(animal.Userid, "mohamed");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.animalId.Should().Be(command.AnimalId);
            animal.IsAdopted.Should().BeTrue();
            await _Repo.Received(1).GetByIdAsync(command.AnimalId);
            await _UserService.Received(2).GetUserByIdAsync(Arg.Any<string>());
            await _PublishEndpoint.Received(1).Publish(Arg.Any<AnimalAdoptedEvent>(), Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task AdoptAnimalHandler_Should_ThrowException_When_AnimalNotFound()
        {
            var command = new AdoptAnimalCommand { AnimalId = 1, AdopterId = "a-123" };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Animal.NotFound");
            await _Repo.DidNotReceive().SaveChangesAsync();
            await _PublishEndpoint.DidNotReceive().Publish(Arg.Any<AnimalAdoptedEvent>(), Arg.Any<CancellationToken>());
            await _UserService.DidNotReceive().GetUserByIdAsync(Arg.Any<string>());

        }
    }

}
