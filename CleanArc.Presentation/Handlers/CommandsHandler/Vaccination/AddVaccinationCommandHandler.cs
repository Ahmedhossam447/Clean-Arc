using CleanArc.Application.Commands.Vaccination;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class AddVaccinationCommandHandler : IRequestHandler<AddVaccinationCommand, Result<VaccinationResponse>>
    {
        private readonly IRepository<Core.Entites.MedicalRecord> _medicalRecordRepository;
        private readonly IRepository<Core.Entites.Vaccination> _vaccinationRepository;
        private readonly IDistributedCache _cache;

        public AddVaccinationCommandHandler(
            IRepository<Core.Entites.MedicalRecord> medicalRecordRepository,
            IRepository<Core.Entites.Vaccination> vaccinationRepository,
            IDistributedCache cache)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _vaccinationRepository = vaccinationRepository;
            _cache = cache;
        }

        public async Task<Result<VaccinationResponse>> Handle(AddVaccinationCommand request, CancellationToken cancellationToken)
        {
            // Get MedicalRecord by AnimalId using repository
            var medicalRecords = await _medicalRecordRepository.GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                return Core.Entites.MedicalRecord.Errors.NotFound;
            }

            var vaccination = new Core.Entites.Vaccination
            {
                MedicalRecordId = medicalRecord.Id,
                Name = request.Name,
                DateGiven = request.DateGiven,
                ExpiryDate = request.ExpiryDate
            };

            await _vaccinationRepository.AddAsync(vaccination);
            await _vaccinationRepository.SaveChangesAsync();

            // Invalidate cache (after write - don't use cancellationToken)
            await _cache.RemoveAsync($"medicalrecord:animal:{request.AnimalId}");
            await _cache.RemoveAsync($"animal:{request.AnimalId}");

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
