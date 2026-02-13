using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAllAnimalsQuery:IRequest<PaginationResponse<ReadAnimalResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
