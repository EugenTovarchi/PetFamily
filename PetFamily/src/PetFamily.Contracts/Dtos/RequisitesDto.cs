using Shared;

namespace PetFamily.Contracts.Dtos;

public  record RequisitesDto(
    string Title,
    string Instruction,
    decimal Value)
{
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace(nameof(Title));

        if (Value < 0)
            return Errors.General.ValueIsInvalid(nameof(Value));

        return Result.Success();
    }
}

