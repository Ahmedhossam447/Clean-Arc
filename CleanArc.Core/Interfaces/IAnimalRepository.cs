using CleanArc.Core.Entites;

namespace CleanArc.Core.Interfaces
{
    public interface IAnimalRepository : IRepository<Animal>
    {
        Task<IEnumerable<Animal>> GetAvailableAnimalsForAdoption(string userid);
    }
}
