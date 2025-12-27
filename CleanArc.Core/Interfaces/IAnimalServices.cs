using CleanArc.Core.Entites;

namespace CleanArc.Core.Interfaces
{
    public interface IAnimalServices
    {
        public Task<IEnumerable<Animal>> GetAvailableAnimalsForAdoption(string userid);
    }
}
