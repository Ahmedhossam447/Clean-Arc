using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Commands.Animal
{
    public class CreateAnimalCommand :IRequest<CreateAnimalResponse>
    {
        public string? Name { get; set; }

        public byte? Age { get; set; }

        public string? Type { get; set; }

        public string? Breed { get; set; }

        public string? Gender { get; set; }

        public string? Photo { get; set; }
        public string? About { get; set; }

        public string? Userid { get; set; }
    }
}
