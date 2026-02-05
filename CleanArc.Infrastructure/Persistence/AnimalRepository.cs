using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence.Data;

namespace CleanArc.Infrastructure.Persistence
{
    public class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public AnimalRepository(AppDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<Animal>> GetAvailableAnimalsForAdoption(string userid)
        {
            if (string.IsNullOrEmpty(userid))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userid));
                
            return await GetAsync(a => a.IsAdopted == false && a.Userid != userid);
        }
    }
}
