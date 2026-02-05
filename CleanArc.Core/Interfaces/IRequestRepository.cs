using CleanArc.Core.Entites;

namespace CleanArc.Core.Interfaces
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<Request?> GetRequestWithAnimalAsync(int requestId, CancellationToken token);

        Task<List<Request>> GetPendingRequestsForAnimalAsync(int animalId, int excludeRequestId, CancellationToken token);

        void RemoveRange(List<Request> requests);
    }
}
