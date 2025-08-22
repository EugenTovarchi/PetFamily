using FluentValidation;
using PetFamily.Contracts.Requests;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerRequestValidator : AbstractValidator <CreateVolunteerRequest>
{
    public CreateVolunteerRequestValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().WithMessage("First name cannot be empty!")
            .MaximumLength(50).WithMessage("First name has to be less than 50 symbols!");

        RuleFor(v => v.LastName).NotEmpty().WithMessage("Last name cannot be empty!")
            .MaximumLength(50).WithMessage("Last name has to be less than 50 symbols!");

        RuleFor(v => v.MiddleName).MaximumLength(50).WithMessage("Middle name has to be less than 50 symbols!");

        RuleFor(v=>v.Phone).NotEmpty().WithMessage("Phone cannot be empty!")
            .MaximumLength(15).WithMessage("Phone has to be less than 15 symbols!");

        RuleFor(v => v.Email).NotEmpty().WithMessage("Email cannot be empty!")
            .MaximumLength(20).WithMessage("Email has to be less than 20 symbols!");

        RuleFor(v => v.VolunteerInfo).NotEmpty().WithMessage("Volunteer info cannot be empty!")
            .MaximumLength(1000).WithMessage("Volunteer info has to be less than 1000 symbols!");

        RuleFor(v => v.ExperienceYears).NotNull().WithMessage("Experience years cannot be empty!")
            .GreaterThanOrEqualTo(0).WithMessage("Experience years has to be more than 0");
    }
}
