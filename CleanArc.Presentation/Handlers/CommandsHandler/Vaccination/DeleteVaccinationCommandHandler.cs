using CleanArc.Application.Commands.Vaccination;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class DeleteVaccinationCommandHandler : IRequestHandler<DeleteVaccinationCommand, Result>
    {
        private readonly IRepository<Core.Entites.Vaccination> _vaccinationRepository;
        private readonly IRepository<Core.Entites.MedicalRecord> _medicalRecordRepository;
        private readonly IDistributedCache _cache;

        public DeleteVaccinationCommandHandler(
            IRepository<Core.Entites.Vaccination> vaccinationRepository,
            IRepository<Core.Entites.MedicalRecord> medicalRecordRepository,
            IDistributedCache cache)
        {
            _vaccinationRepository = vaccinationRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _cache = cache;
        }

        public async Task<Result> Handle(DeleteVaccinationCommand request, CancellationToken cancellationToken)
        {
            var vaccination = await _vaccinationRepository.GetByIdAsync(request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                return Core.Entites.Vaccination.Errors.NotFound;
            }

            // Get MedicalRecord to find AnimalId for cache invalidation
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(vaccination.MedicalRecordId, cancellationToken);
            var animalId = medicalRecord?.AnimalId ?? 0;

            await _vaccinationRepository.Delete(request.VaccinationId);
            await _vaccinationRepository.SaveChangesAsync();

            // Invalidate cache (after write - don't use cancellationToken)
            if (animalId > 0)
            {
                await _cache.RemoveAsync($"medicalrecord:animal:{animalId}");
                await _cache.RemoveAsync($"animal:{animalId}");
            }

            return Result.Success();
        }
    }
}
