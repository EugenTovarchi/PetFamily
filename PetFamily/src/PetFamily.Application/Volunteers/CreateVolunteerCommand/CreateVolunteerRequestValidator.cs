using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerRequestValidator : AbstractValidator<CreateVolunteerRequest>
{
    public CreateVolunteerRequestValidator()
    {
        RuleFor(c => c)
            .MustBeValueObject(request => string.IsNullOrEmpty(request.MiddleName)
            ? FullName.Create(request.FirstName, request.LastName)
            : FullName.CreateWithMiddle(request.FirstName, request.LastName, request.MiddleName));

        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Email).MustBeValueObject(Email.Create);

        RuleFor(c => c.VolunteerInfo).NotEmpty().WithMessage("Volunteer info cannot be empty!")
            .MaximumLength(1000).WithMessage("Volunteer info has to be less than 1000 symbols!");

        RuleFor(c => c.ExperienceYears).NotNull().WithMessage("Experience years cannot be empty!")
            .GreaterThanOrEqualTo(0).WithMessage("Experience years has to be more than 0");

        //через SetValidator почему то не хочет(не видит метод Create у сущности)
        RuleForEach(c => c.VolunteerSocialMediaDtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
           .When(c => c.VolunteerSocialMediaDtos != null);

        //попробовал разные варианты
        RuleForEach(c => c.RequisitesDtos).Must(dto => dto.Validate().IsSuccess);
    }
}
