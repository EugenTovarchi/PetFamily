using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetValidator :AbstractValidator<AddPetCommand>
{
    public AddPetValidator()
    {
        RuleFor(p => p.VolunteerId).NotEmpty().WithError(Errors.General.NotFoundValue());

        RuleFor(p => p.PetName)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("PetName"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("PetName"));

        RuleFor(p => p.Description)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("Description"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("Description"));

        RuleFor(p => p.HealthInfo)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("HealthInfo"))
         .MaximumLength(1000).WithError(Errors.Validation.RecordIsInvalid("HealthInfo"));

        RuleFor(c => c.Address)
            .MustBeValueObject(address =>
                address.Flat.HasValue
                    ? Address.CreateWithFlat(
                        address.City,
                        address.Street,
                        address.House,
                        address.Flat.Value)
                    : Address.Create(
                        address.City,
                        address.Street,
                        address.House)
            );

        RuleFor(c => c.Vaccinated)
            .NotNull().WithError(Errors.General.ValueIsRequired("Vaccinated"));

        RuleFor(c => c.Height)
            .GreaterThan(0).WithError(Errors.General.ValueMustBePositive("Height"))
            .LessThanOrEqualTo(610).WithError(Errors.General.ValueIsInvalid("Height"));

        RuleFor(c => c.Weight)
            .GreaterThan(0).WithError(Errors.General.ValueMustBePositive("Weight"))
            .LessThanOrEqualTo(1800).WithError(Errors.General.ValueIsInvalid("Weight"));

        RuleFor(c => c.SpeciesId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("SpeciesId"))
            .Must(id => id != Guid.Empty).WithError(Errors.General.ValueIsInvalid("SpeciesId"));

        RuleFor(c => c.BreedId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("BreedId"))
            .Must(id => id != Guid.Empty).WithError(Errors.General.ValueIsInvalid("BreedId"));

        RuleFor(c => c.Color)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("Color"))
            .MaximumLength(30).WithError(Errors.General.ValueIsInvalid("Color"))
            .Must(IsValidPetColor).WithError(Errors.General.ValueIsInvalid("Color"));

        RuleFor(c => c.PetStatus)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("PetStatus"))
            .Must(status => !string.IsNullOrWhiteSpace(status) &&
                 new[] { "LookingTreatment", "LookingHome", "HasHome" } 
                    .Contains(status))
            .WithError(Errors.General.ValueIsInvalid("PetStatus"));
    }
    private bool IsValidPetColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        var russianColor = color.ToLower().Trim();

        return russianColor switch
        {
            "чёрный" or "черный" => true,
            "белый" => true,
            "коричневый" => true,
            "серый" => true,
            "рыжий" => true,
            "кремовый" => true,
            "золотистый" => true,
            "пятнистый" => true,
            "полосатый" => true,
            "смешанный" => true,
            "чёрно-белый" or "черно-белый" => true,
            _ => false
        };
    }

}
 