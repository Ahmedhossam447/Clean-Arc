using CleanArc.Core.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Queries
{
    public class GetCarById : IRequest<Car>
    {
        public int Id { get; set; }
        public GetCarById(int id)
        {
            Id = id;
        }
    }
}
