using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateMainInfoCommand;

public class UpdateMainInfoRequestValidator : AbstractValidator<UpdateMainInfoRequest>
{
    public UpdateMainInfoRequestValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));
    }
}
public class UpdateMainInfoDtoValidator : AbstractValidator<UpdateMainInfoDto>
{
    public UpdateMainInfoDtoValidator()
    {
        RuleFor(c => c.FullName)
        .MustBeValueObject(fullNameRequest =>
        string.IsNullOrWhiteSpace(fullNameRequest.MiddleName)
            ? FullName.Create(fullNameRequest.FirstName, fullNameRequest.LastName)
            : FullName.CreateWithMiddle(fullNameRequest.FirstName, fullNameRequest.LastName, fullNameRequest.MiddleName));

        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Email).MustBeValueObject(Email.Create);

        RuleFor(c => c.VolunteerInfo)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerInfo"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("VolunteerInfo"));

        RuleFor(c => c.ExperienceYears)
         .GreaterThanOrEqualTo(0).WithError(Errors.General.ValueMustBePositive("ExperienceYears"));
    }
}
