using CleanArc.Core.Entites;
using MediatR;

namespace CleanArc.Application.Commands
{
    public class AddCarCommand :IRequest<Car>
    {
        public int Id { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? Color { get; set; }
       public string? LicensePlate { get; set; }
        public AddCarCommand( string? make, string? model, int year, string? color, string? licensePlate)
        {
            Make = make;
            Model = model;
            Year = year;
            Color = color;
            LicensePlate = licensePlate;
        }

    }
}
