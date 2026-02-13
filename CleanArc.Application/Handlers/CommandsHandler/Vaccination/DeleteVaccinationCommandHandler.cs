using CleanArc.Application.Commands.Vaccination;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class DeleteVaccinationCommandHandler : IRequestHandler<DeleteVaccinationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public DeleteVaccinationCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result> Handle(DeleteVaccinationCommand request, CancellationToken cancellationToken)
        {
            var vaccinationRepo = _unitOfWork.Repository<Core.Entities.Vaccination>();
            var vaccination = await vaccinationRepo.GetByIdAsync(request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                return Core.Entities.Vaccination.Errors.NotFound;
            }

            var medicalRecord = await _unitOfWork.Repository<Core.Entities.MedicalRecord>().GetByIdAsync(vaccination.MedicalRecordId, cancellationToken);
            var animalId = medicalRecord?.AnimalId ?? 0;

            await vaccinationRepo.Delete(request.VaccinationId);
            await _unitOfWork.SaveChangesAsync();

            if (animalId > 0)
            {
                await _cache.RemoveAsync($"medicalrecord:animal:{animalId}");
                await _cache.RemoveAsync($"animal:{animalId}");
            }

            return Result.Success();
        }
    }
}
