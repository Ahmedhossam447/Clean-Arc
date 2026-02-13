using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, Result<RequestResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public UpdateRequestCommandHandler(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<RequestResponse>> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.Repository<Request>();
            var request = await requestRepo.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Request.Errors.NotFound;

            var oldRequesterId = request.RequesterId;

            if (!string.IsNullOrEmpty(command.OwnerId))
                request.OwnerId = command.OwnerId;

            if (!string.IsNullOrEmpty(command.RequesterId))
                request.RequesterId = command.RequesterId;

            if (command.AnimalId.HasValue)
                request.AnimalId = command.AnimalId.Value;

            if (!string.IsNullOrEmpty(command.Status))
                request.Status = command.Status;

            requestRepo.Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{oldRequesterId}");
            if (oldRequesterId != request.RequesterId)
                await _cache.RemoveAsync($"requests:user:{request.RequesterId}");

            return new RequestResponse
            {
                Id = request.Id,
                OwnerId = request.OwnerId,
                RequesterId = request.RequesterId,
                AnimalId = request.AnimalId,
                AnimalName = request.Animal?.Name,
                Status = request.Status
            };
        }
    }
}
