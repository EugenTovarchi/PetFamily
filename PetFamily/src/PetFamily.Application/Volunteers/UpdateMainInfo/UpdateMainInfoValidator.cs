using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoCommand>
{
    public UpdateMainInfoCommandValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));
        RuleFor(c => c.Request.VolunteerInfo)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerInfo"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("VolunteerInfo"));

        RuleFor(c => c.Request.ExperienceYears)
         .GreaterThanOrEqualTo(1).WithError(Errors.General.ValueMustBePositive("ExperienceYears"));

        RuleFor(c => c.Request.FullName)
        .MustBeValueObject(fullNameRequest =>
        string.IsNullOrWhiteSpace(fullNameRequest.MiddleName)
            ? FullName.Create(fullNameRequest.FirstName, fullNameRequest.LastName)
            : FullName.CreateWithMiddle(fullNameRequest.FirstName, fullNameRequest.LastName, fullNameRequest.MiddleName));

        RuleFor(c => c.Request.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Request.Email).MustBeValueObject(Email.Create);
    }
}


