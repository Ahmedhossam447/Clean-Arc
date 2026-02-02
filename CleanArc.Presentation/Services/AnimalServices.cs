using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;

namespace CleanArc.Application.Services
{
    public class AnimalServices : IAnimalServices
    {
        private readonly IRepository<Animal> _animalRepository;
        
        public AnimalServices(IRepository<Animal> animalRepository)
        {
            _animalRepository = animalRepository;
        }
        
        public async Task<IEnumerable<Animal>> GetAvailableAnimalsForAdoption(string userid)
        {
            if (string.IsNullOrEmpty(userid))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userid));
                
            var animals = await _animalRepository.GetAsync(a => a.IsAdopted == false
                                                           && a.Userid != userid);

            return animals;
        }
    }
}
