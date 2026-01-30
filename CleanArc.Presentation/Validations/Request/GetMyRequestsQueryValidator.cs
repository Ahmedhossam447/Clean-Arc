using CleanArc.Application.Queries.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class GetMyRequestsQueryValidator : AbstractValidator<GetMyRequestsQuery>
    {
        public GetMyRequestsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
