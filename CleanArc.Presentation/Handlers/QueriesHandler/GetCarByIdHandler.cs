using CleanArc.Application.Queries;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Handlers.QueriesHandler
{
    public class GetCarByIdHandler : IRequestHandler<GetCarById, Car>
    {
        private readonly ICarServices _carServices;
        public GetCarByIdHandler(ICarServices carServices)
        {
            _carServices = carServices;
        }
        public async Task<Car> Handle(GetCarById request, CancellationToken cancellationToken)
        {
            return await _carServices.GetCarById(request.Id);
        }
    }
}
