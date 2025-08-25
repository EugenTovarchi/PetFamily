using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared;
using Shared;
using System.Reflection.Metadata;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerRequestValidator : AbstractValidator<CreateVolunteerRequest>
{
    public CreateVolunteerRequestValidator()
    {
        RuleFor(c => c.FullName)
            .MustBeValueObject(c =>
            string.IsNullOrWhiteSpace(c.MiddleName)
            ? FullName.Create(c.FirstName, c.LastName)
            : FullName.CreateWithMiddle(c.FirstName, c.LastName, c.MiddleName));

        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Email).MustBeValueObject(Email.Create);

        RuleFor(c => c.VolunteerInfo)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerInfo"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("VolunteerInfo"));

        RuleFor(c => c.ExperienceYears)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("ExperienceYears"))
         .GreaterThanOrEqualTo(0).WithError(Errors.General.ValueMustBePositive("ExperienceYears"));

        RuleForEach(c => c.VolunteerSocialMediaDtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
           .When(c => c.VolunteerSocialMediaDtos != null);

        //попробовал разные варианты
        RuleForEach(c => c.RequisitesDtos).Must(dto => dto.Validate().IsSuccess);
    }
}
