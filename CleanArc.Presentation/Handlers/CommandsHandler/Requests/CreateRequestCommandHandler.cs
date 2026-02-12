using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Result<CreateRequestResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public CreateRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<CreateRequestResponse>> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
        {
            var animal = await _unitOfWork.Repository<Animal>().GetByIdAsync(command.AnimalId, cancellationToken);
            if (animal == null)
            {
                return Animal.Errors.NotFound;
            }

            if (animal.IsAdopted)
            {
                return Animal.Errors.AlreadyAdopted;
            }

            if (animal.Userid == command.RequesterId)
            {
                return Request.Errors.CannotRequestOwnAnimal;
            }

            var requestRepo = _unitOfWork.Repository<Request>();
            var existingRequests = await requestRepo.GetAsync(
                r => r.AnimalId == command.AnimalId && r.Useridreq == command.RequesterId, cancellationToken);
            
            if (existingRequests.Any())
            {
                return Request.Errors.AlreadyExists;
            }

            var request = new Request
            {
                Userid = animal.Userid,
                Useridreq = command.RequesterId,
                AnimalId = command.AnimalId,
                Status = "Pending"
            };

            await requestRepo.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{command.RequesterId}");

            var response = new CreateRequestResponse
            {
                Reqid = request.Reqid,
                Userid = request.Userid,
                Useridreq = request.Useridreq,
                AnimalId = request.AnimalId,
                Status = request.Status
            };

            return response;
        }
    }
}
