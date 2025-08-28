using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateRequisitesCommand;

public class UpdateRequsitesRequestValidator : AbstractValidator<UpdateRequisitesRequest>
{
    public UpdateRequsitesRequestValidator()
    {
        RuleFor(u => u.Id).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("VolunteerId"));
    }
}
public class UpdateRequisiteDtoValidator : AbstractValidator<UpdateRequisitesDto>
{
    public UpdateRequisiteDtoValidator()
    {
        RuleForEach(c => c.Dtos).MustBeValueObject(dto => Requisites.Create(dto.Title, dto.Instruction, dto.Value))
            .When(c => c != null);
    }
}
