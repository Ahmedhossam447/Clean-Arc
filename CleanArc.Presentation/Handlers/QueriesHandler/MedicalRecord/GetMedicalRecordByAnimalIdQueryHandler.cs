using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Application.Queries.MedicalRecord;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.MedicalRecord
{
    public class GetMedicalRecordByAnimalIdQueryHandler : IRequestHandler<GetMedicalRecordByAnimalIdQuery, Result<MedicalRecordResponse>>
    {
        private readonly IRepository<Core.Entites.MedicalRecord> _medicalRecordRepository;
        private readonly IRepository<Core.Entites.Vaccination> _vaccinationRepository;

        public GetMedicalRecordByAnimalIdQueryHandler(
            IRepository<Core.Entites.MedicalRecord> medicalRecordRepository,
            IRepository<Core.Entites.Vaccination> vaccinationRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _vaccinationRepository = vaccinationRepository;
        }

        public async Task<Result<MedicalRecordResponse>> Handle(GetMedicalRecordByAnimalIdQuery request, CancellationToken cancellationToken)
        {
            // Get MedicalRecord by AnimalId using repository
            var medicalRecords = await _medicalRecordRepository.GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                return Core.Entites.MedicalRecord.Errors.NotFound;
            }

            // Get Vaccinations separately using repository
            var vaccinations = await _vaccinationRepository.GetAsync(
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
