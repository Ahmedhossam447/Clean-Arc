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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public DeleteRequestCommandHandler(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result> Handle(DeleteRequestCommand command, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.Repository<Request>();
            var request = await requestRepo.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.Useridreq != command.UserId)
                return Result.Failure(Request.Errors.Unauthorized);

            await requestRepo.Delete(command.RequestId);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{command.UserId}");

            return Result.Success();
        }
    }
}
