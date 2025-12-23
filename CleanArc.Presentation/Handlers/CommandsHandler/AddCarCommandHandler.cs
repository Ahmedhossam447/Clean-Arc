using CleanArc.Application.Commands;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Handlers.CommandsHandler
{
    public class AddCarCommandHandler : IRequestHandler<AddCarCommand, Car>
    {
        private readonly ICarServices _servcies;
        public AddCarCommandHandler(ICarServices servcies)
        {
            _servcies = servcies;
        }
        public async Task<Car> Handle(AddCarCommand request, CancellationToken cancellationToken)
        {
            var car = new Car
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                Color = request.Color,
                LicensePlate = request.LicensePlate
            };
            return await _servcies.Create(car);
        }
    }
}
