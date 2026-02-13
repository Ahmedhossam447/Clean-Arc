using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Application.Queries.MedicalRecord;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.MedicalRecord
{
    public class GetMedicalRecordByAnimalIdQueryHandler : IRequestHandler<GetMedicalRecordByAnimalIdQuery, Result<MedicalRecordResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMedicalRecordByAnimalIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<MedicalRecordResponse>> Handle(GetMedicalRecordByAnimalIdQuery request, CancellationToken cancellationToken)
        {
            var medicalRecords = await _unitOfWork.Repository<Core.Entities.MedicalRecord>().GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                return Core.Entities.MedicalRecord.Errors.NotFound;
            }

            var vaccinations = await _unitOfWork.Repository<Core.Entities.Vaccination>().GetAsync(
                v => v.MedicalRecordId == medicalRecord.Id, cancellationToken);

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
