using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArc.Infrastructure.Persistence
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Request?> GetRequestWithAnimalAsync(int requestId, CancellationToken token)
        {
            return await _context.Requests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Reqid == requestId, token);
        }

        public async Task<List<Request>> GetPendingRequestsForAnimalAsync(int animalId, int excludeRequestId, CancellationToken token)
        {
            return await _context.Requests
                .Where(r => r.AnimalId == animalId 
                         && r.Reqid != excludeRequestId 
                         && r.Status == "Pending")
                .ToListAsync(token);
        }

        public void RemoveRange(List<Request> requests)
        {
            _context.Requests.RemoveRange(requests);
        }
    }
}
