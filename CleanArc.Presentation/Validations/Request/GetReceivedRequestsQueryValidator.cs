using CleanArc.Application.Queries.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class GetReceivedRequestsQueryValidator : AbstractValidator<GetReceivedRequestsQuery>
    {
        public GetReceivedRequestsQueryValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required.");
        }
    }
}
