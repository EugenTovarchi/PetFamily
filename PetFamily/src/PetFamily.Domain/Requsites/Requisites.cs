using Shared;

namespace PetFamily.Domain.Requsites;

public sealed record Requisites(
    string Title,
    string Instruction,
    decimal Value)
{
    public static Result<Requisites> Create(string title, string instruction, decimal value)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("title");

        var processedInstruction = instruction?.Trim() ?? string.Empty;

        if (value < 0)
            return Errors.General.ValueMustBePositive("requsitesValue");

        return new Requisites(title.Trim(), processedInstruction, value);
    }
}
