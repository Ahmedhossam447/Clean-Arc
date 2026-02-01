using CleanArc.Application.Commands.Vaccination;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class UpdateVaccinationCommandHandler : IRequestHandler<UpdateVaccinationCommand, Result<VaccinationResponse>>
    {
        private readonly IRepository<Core.Entites.Vaccination> _vaccinationRepository;
        private readonly IRepository<Core.Entites.MedicalRecord> _medicalRecordRepository;
        private readonly IDistributedCache _cache;

        public UpdateVaccinationCommandHandler(
            IRepository<Core.Entites.Vaccination> vaccinationRepository,
            IRepository<Core.Entites.MedicalRecord> medicalRecordRepository,
            IDistributedCache cache)
        {
            _vaccinationRepository = vaccinationRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _cache = cache;
        }

        public async Task<Result<VaccinationResponse>> Handle(UpdateVaccinationCommand request, CancellationToken cancellationToken)
        {
            var vaccination = await _vaccinationRepository.GetByIdAsync(request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                return Core.Entites.Vaccination.Errors.NotFound;
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.Name))
                vaccination.Name = request.Name;

            if (request.DateGiven.HasValue)
                vaccination.DateGiven = request.DateGiven.Value;

            if (request.ExpiryDate.HasValue)
                vaccination.ExpiryDate = request.ExpiryDate.Value;

            _vaccinationRepository.Update(vaccination);
            await _vaccinationRepository.SaveChangesAsync();

            // Get MedicalRecord to find AnimalId for cache invalidation
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(vaccination.MedicalRecordId, cancellationToken);
            var animalId = medicalRecord?.AnimalId ?? 0;

            // Invalidate cache (after write - don't use cancellationToken)
            if (animalId > 0)
            {
                await _cache.RemoveAsync($"medicalrecord:animal:{animalId}");
                await _cache.RemoveAsync($"animal:{animalId}");
            }

            var response = new VaccinationResponse
            {
                Id = vaccination.Id,
                Name = vaccination.Name,
                DateGiven = vaccination.DateGiven,
                ExpiryDate = vaccination.ExpiryDate,
                IsExpired = vaccination.IsExpired
            };

            return response;
        }
    }
}
