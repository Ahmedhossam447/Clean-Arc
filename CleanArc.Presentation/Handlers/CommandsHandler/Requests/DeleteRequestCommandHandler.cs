using CleanArc.Application.Commands.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, Result>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IDistributedCache _cache;

        public DeleteRequestCommandHandler(IRepository<Request> requestRepository, IDistributedCache cache)
        {
            _requestRepository = requestRepository;
            _cache = cache;
        }

        public async Task<Result> Handle(DeleteRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.Useridreq != command.UserId)
                return Result.Failure(Request.Errors.Unauthorized);

            await _requestRepository.Delete(command.RequestId);
            await _requestRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{command.UserId}");

            return Result.Success();
        }
    }
}
