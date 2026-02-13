using CleanArc.Application.Commands.Vaccination;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class AddVaccinationCommandHandler : IRequestHandler<AddVaccinationCommand, Result<VaccinationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public AddVaccinationCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<VaccinationResponse>> Handle(AddVaccinationCommand request, CancellationToken cancellationToken)
        {
            var medicalRecords = await _unitOfWork.Repository<Core.Entities.MedicalRecord>().GetAsync(
                m => m.AnimalId == request.AnimalId, cancellationToken);
            var medicalRecord = medicalRecords.FirstOrDefault();

            if (medicalRecord == null)
            {
                return Core.Entities.MedicalRecord.Errors.NotFound;
            }

            var vaccination = new Core.Entities.Vaccination
            {
                MedicalRecordId = medicalRecord.Id,
                Name = request.Name,
                DateGiven = request.DateGiven,
                ExpiryDate = request.ExpiryDate
            };

            await _unitOfWork.Repository<Core.Entities.Vaccination>().AddAsync(vaccination);
            await _unitOfWork.SaveChangesAsync();

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
