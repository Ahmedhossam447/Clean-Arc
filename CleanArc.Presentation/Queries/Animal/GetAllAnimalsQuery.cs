using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAllAnimalsQuery:IRequest<GetAllAnimalsResponse>
    {
    }
}
