using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerRequestValidator : AbstractValidator<CreateVolunteerRequest>
{
    public CreateVolunteerRequestValidator()
    {
        RuleFor(c => c)
            .Custom((request, context) =>
            {
                var fullNameResult = request.MiddleName == null
                    ? FullName.Create(request.FirstName, request.LastName)
                    : FullName.CreateWithMiddle(request.FirstName, request.LastName, request.MiddleName);

                if (fullNameResult.IsFailure)
                {
                    foreach (var error in fullNameResult.Error.ToFailure())
                    {
                        context.AddFailure(error.Message);
                    }
                }
            });

        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Email).MustBeValueObject(Email.Create);

        RuleFor(c => c.VolunteerInfo).NotEmpty().WithMessage("Volunteer info cannot be empty!") //Сделать это VO?
            .MaximumLength(1000).WithMessage("Volunteer info has to be less than 1000 symbols!");

        RuleFor(c => c.ExperienceYears).NotNull().WithMessage("Experience years cannot be empty!")
            .GreaterThanOrEqualTo(0).WithMessage("Experience years has to be more than 0");
    }
}
