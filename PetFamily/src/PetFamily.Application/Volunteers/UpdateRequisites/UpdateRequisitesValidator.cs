using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Application.Volunteers.UpdateRequisites;

public class UpdateRequsitesCommandValidator : AbstractValidator<UpdateRequisitesCommand>
{
    public UpdateRequsitesCommandValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));

        RuleForEach(c => c.UpdateRequisitesDto.Dtos).MustBeValueObject(dto => Requisites.Create(dto.Title, dto.Instruction, dto.Value))
            .When(c => c != null);
    }
}

//public class UpdateRequisiteDtoValidator : AbstractValidator<UpdateRequisitesRequest>
//{
//    public UpdateRequisiteDtoValidator()
//    {
//        RuleForEach(c => c.Dtos).MustBeValueObject(dto => Requisites.Create(dto.Title, dto.Instruction, dto.Value))
//            .When(c => c != null);
//    }
//}
