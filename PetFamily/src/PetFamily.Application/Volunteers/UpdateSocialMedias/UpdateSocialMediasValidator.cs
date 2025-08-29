using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Dtos;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

public class UpdateSocialMediasRequestValidator : AbstractValidator<UpdateSocialMediaRequest>
{
    public UpdateSocialMediasRequestValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));
    }
}
public  class UpdateSocialMediasDtoValidator : AbstractValidator<UpdateSocialMediaDto>
{
    public UpdateSocialMediasDtoValidator()
    {
        RuleForEach(c => c.Dtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
            .When(c => c != null);
    }
}
