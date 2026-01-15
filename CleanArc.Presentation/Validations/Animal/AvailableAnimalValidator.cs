using CleanArc.Application.Queries.Animal;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Validations.Animal
{
    public class AvailableAnimalValidator : AbstractValidator<GetAvailableAnimalsForAdoptionQuery>
    {
        public AvailableAnimalValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
