using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared;

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
         .NotEmpty().WithError("value.is.invalid", "Volunteer info cannot be empty!")
         .MaximumLength(1000).WithError("value.is.invalid", "Volunteer info has to be less than 1000 symbols!");

        RuleFor(c => c.ExperienceYears)
         .NotEmpty().WithError("value.is.invalid", "ExperienceYears  cannot be empty!")
         .GreaterThanOrEqualTo(0).WithError("value.is.invalid", "Experience years has to be possitive");

        RuleForEach(c => c.VolunteerSocialMediaDtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
           .When(c => c.VolunteerSocialMediaDtos != null);

        //попробовал разные варианты
        RuleForEach(c => c.RequisitesDtos).Must(dto => dto.Validate().IsSuccess);
    }
}
