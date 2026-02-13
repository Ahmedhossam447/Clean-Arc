using CleanArc.Application.Commands.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class UpdateRequestCommandValidator : AbstractValidator<UpdateRequestCommand>
    {
        private static readonly string[] ValidStatuses = { "Pending", "Approved", "Rejected", "Cancelled" };

        public UpdateRequestCommandValidator()
        {
            RuleFor(x => x.RequestId)
                .GreaterThan(0).WithMessage("Request ID must be greater than zero.");

            RuleFor(x => x.AnimalId)
                .GreaterThan(0).When(x => x.AnimalId.HasValue)
                .WithMessage("Animal ID must be greater than zero.");

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrEmpty(status) || ValidStatuses.Contains(status))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");
        }
    }
}
