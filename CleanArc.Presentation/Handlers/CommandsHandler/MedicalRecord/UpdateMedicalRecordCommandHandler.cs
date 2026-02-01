using CleanArc.Application.Commands.MedicalRecord;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.MedicalRecord
{
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, MedicalRecordResponse>
    {
        private readonly IRepository<Core.Entites.MedicalRecord> _medicalRecordRepository;
        private readonly IRepository<Core.Entites.Vaccination> _vaccinationRepository;
        private readonly IDistributedCache _cache;

        public UpdateMedicalRecordCommandHandler(
            IRepository<Core.Entites.MedicalRecord> medicalRecordRepository,
            IRepository<Core.Entites.Vaccination> vaccinationRepository,
            IDistributedCache cache)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _vaccinationRepository = vaccinationRepository;
            _cache = cache;
        }

        public async Task<MedicalRecordResponse> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // Get MedicalRecord by AnimalId using repository
            var medicalRecords = await _medicalRecordRepository.GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                throw new KeyNotFoundException($"Medical record for Animal ID {request.AnimalId} not found.");
            }

            // Update only provided fields
            if (request.Weight.HasValue)
                medicalRecord.Weight = request.Weight.Value;
            
            if (request.Height.HasValue)
                medicalRecord.Height = request.Height.Value;
            
            if (request.BloodType != null)
                medicalRecord.BloodType = request.BloodType;
            
            if (request.MedicalHistoryNotes != null)
                medicalRecord.MedicalHistoryNotes = request.MedicalHistoryNotes;
            
            if (request.Injuries != null)
                medicalRecord.Injuries = request.Injuries;
            
            if (request.Status != null)
                medicalRecord.Status = request.Status;

            _medicalRecordRepository.Update(medicalRecord);
            await _medicalRecordRepository.SaveChangesAsync();

            // Get Vaccinations separately for response
            var vaccinations = await _vaccinationRepository.GetAsync(
                v => v.MedicalRecordId == medicalRecord.Id, cancellationToken);

            // Invalidate cache (after write - don't use cancellationToken)
            await _cache.RemoveAsync($"medicalrecord:animal:{request.AnimalId}");
            await _cache.RemoveAsync($"animal:{request.AnimalId}");

            var response = new MedicalRecordResponse
            {
                Id = medicalRecord.Id,
                AnimalId = medicalRecord.AnimalId,
                Weight = medicalRecord.Weight,
                Height = medicalRecord.Height,
                BloodType = medicalRecord.BloodType,
                MedicalHistoryNotes = medicalRecord.MedicalHistoryNotes,
                Injuries = medicalRecord.Injuries,
                Status = medicalRecord.Status,
                Vaccinations = vaccinations.Select(v => new VaccinationResponse
                {
                    Id = v.Id,
                    Name = v.Name,
                    DateGiven = v.DateGiven,
                    ExpiryDate = v.ExpiryDate,
                    IsExpired = v.IsExpired
                }).ToList()
            };

            return response;
        }
    }
}
