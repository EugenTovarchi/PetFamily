using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

public class UpdateSocialMediasCommandValidator : AbstractValidator<UpdateSocialMediaCommand>
{
    public UpdateSocialMediasCommandValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));
        RuleForEach(c => c.SocialMedias.Dtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
            .When(c => c != null);
    }
}
//public  class UpdateSocialMediasDtoValidator : AbstractValidator<Contracts.Requests.Volunteers.UpdateSocialMediaRequest>
//{
//    public UpdateSocialMediasDtoValidator()
//    {
//        RuleForEach(c => c.Dtos).MustBeValueObject(dto => VolunteerSocialMedia.Create(dto.Title, dto.Url))
//            .When(c => c != null);
//    }
//}
