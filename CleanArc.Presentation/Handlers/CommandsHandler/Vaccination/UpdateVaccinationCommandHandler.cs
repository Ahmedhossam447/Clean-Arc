using CleanArc.Application.Commands.Vaccination;
using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Vaccination
{
    public class UpdateVaccinationCommandHandler : IRequestHandler<UpdateVaccinationCommand, Result<VaccinationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public UpdateVaccinationCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<VaccinationResponse>> Handle(UpdateVaccinationCommand request, CancellationToken cancellationToken)
        {
            var vaccinationRepo = _unitOfWork.Repository<Core.Entites.Vaccination>();
            var vaccination = await vaccinationRepo.GetByIdAsync(request.VaccinationId, cancellationToken);

            if (vaccination == null)
            {
                return Core.Entites.Vaccination.Errors.NotFound;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                vaccination.Name = request.Name;

            if (request.DateGiven.HasValue)
                vaccination.DateGiven = request.DateGiven.Value;

            if (request.ExpiryDate.HasValue)
                vaccination.ExpiryDate = request.ExpiryDate.Value;

            vaccinationRepo.Update(vaccination);
            await _unitOfWork.SaveChangesAsync();

            var medicalRecord = await _unitOfWork.Repository<Core.Entites.MedicalRecord>().GetByIdAsync(vaccination.MedicalRecordId, cancellationToken);
            var animalId = medicalRecord?.AnimalId ?? 0;

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
