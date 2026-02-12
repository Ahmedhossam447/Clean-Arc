using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Animals
{
    public class GetAnimalByIdQueryHandler : IRequestHandler<GetAnimalByIdQuery, Result<ReadAnimalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAnimalByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReadAnimalResponse>> Handle(GetAnimalByIdQuery request, CancellationToken cancellationToken)
        {
            var animal = await _unitOfWork.Repository<Animal>().GetByIdAsync(request.AnimalId, cancellationToken);
            if (animal == null)
            {
                return Animal.Errors.NotFound;
            }

            var response = new ReadAnimalResponse
            {
                AnimalId = animal.AnimalId,
                Name = animal.Name,
                Age = animal.Age,
                Type = animal.Type,
                Breed = animal.Breed,
                Gender = animal.Gender,
                Photo = animal.Photo,
                About = animal.About,
                IsAdopted = animal.IsAdopted,
                Userid = animal.Userid
            };
            return response;
        }
    }
}
