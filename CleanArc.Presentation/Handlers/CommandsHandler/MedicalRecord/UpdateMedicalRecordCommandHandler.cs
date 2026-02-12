using CleanArc.Application.Commands.MedicalRecord;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.MedicalRecord
{
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, MedicalRecordResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public UpdateMedicalRecordCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<MedicalRecordResponse> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecordRepo = _unitOfWork.Repository<Core.Entites.MedicalRecord>();
            var medicalRecords = await medicalRecordRepo.GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                throw new KeyNotFoundException($"Medical record for Animal ID {request.AnimalId} not found.");
            }

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

            medicalRecordRepo.Update(medicalRecord);
            await _unitOfWork.SaveChangesAsync();

            var vaccinations = await _unitOfWork.Repository<Core.Entites.Vaccination>().GetAsync(
                v => v.MedicalRecordId == medicalRecord.Id, cancellationToken);

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
