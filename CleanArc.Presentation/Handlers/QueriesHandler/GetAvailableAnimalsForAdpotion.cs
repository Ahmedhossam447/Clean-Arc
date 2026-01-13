using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries;
using CleanArc.Core.Interfaces;
using MediatR;
using System.Linq;


namespace CleanArc.Application.Handlers.QueriesHandler
{
    public class GetAvailableAnimalsForAdoption : IRequestHandler<GetAvailableAnimalsForAdoptionQuery, GetAvailableAnimalsForAdoptionResponse>
    {
        private readonly IAnimalServices _animalServices;
        public GetAvailableAnimalsForAdoption(IAnimalServices animalServices)
        {
            _animalServices = animalServices;
        }
        public async Task<GetAvailableAnimalsForAdoptionResponse> Handle(GetAvailableAnimalsForAdoptionQuery request, CancellationToken cancellationToken)
        {
            var animals = await _animalServices.GetAvailableAnimalsForAdoption(request.UserId);
            var response = new GetAvailableAnimalsForAdoptionResponse
            {
                Animals = animals.Select(a => new ReadAnimalResponse
                {
                    AnimalId = a.AnimalId,
                    Name = a.Name,
                    Type = a.Type,
                    Age = a.Age,
                    Breed = a.Breed,
                    Photo = a.Photo,
                    About = a.About,
                    Gender = a.Gender,
                    IsAdopted = a.IsAdopted,
                    Userid = a.Userid
                }).ToList()
            };

            return response;
        }
    }
}
