using CleanArc.Application.Queries.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class GetRequestByIdQueryValidator : AbstractValidator<GetRequestByIdQuery>
    {
        public GetRequestByIdQueryValidator()
        {
            RuleFor(x => x.RequestId)
                .GreaterThan(0).WithMessage("Request ID must be greater than zero.");
        }
    }
}
