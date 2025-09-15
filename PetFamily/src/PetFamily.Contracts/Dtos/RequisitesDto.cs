using CSharpFunctionalExtensions;
using Shared;
using Result = CSharpFunctionalExtensions.Result;

namespace PetFamily.Contracts.Dtos;

public  record RequisitesDto(
    string Title,
    string Instruction,
    decimal Value)
{
    public UnitResult<Error> Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("Title");

        if (Value < 0)
            return Errors.General.ValueIsInvalid("Value");

        return Result.Success<Error>();
    }
}

