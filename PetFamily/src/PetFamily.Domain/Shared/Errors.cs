namespace PetFamily.Domain.Shared;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid.");
        }
        public static Error NotFound(Guid? id = null)
        {
            var forId = id == null ? " " : $"for id '{id}'";
            return Error.NotFound("record.not.found", $"record not found {forId}");
        }

        public static Error NotFoundValue(string? field = null)
        {
            return Error.NotFound("value.not.found", $"velue not found {field}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            var label = name == null ? " " : " " + name + " ";
            return Error.Validation("length.is.invalid", $"invalid {label} length");
        }

        public static Error ValueIsEmptyOrWhiteSpace(string field)
        {
            return Error.Validation("value.is.empty", $"invalid {field} is empty or white space");
        }

        public static Error Duplicate(string field)
        {
            return Error.Conflict("field.duplicate", $"{field} already exists");
        }

        public static Error EmptyId(Guid id)
        {
            return Error.Validation("id.empty", $"{id} cannot be empty Guid");
        }

        public static Error ValueIsZero(string field)
        {
            return Error.Validation("value.is.zero", $"{field} can not be zero.");
        }

    }

    public static class Volunteer
    {
        public static Error ValueMustBePositive(string field)
        {
            return Error.Validation("value.not.positive", $"{field} can not be negative.");
        }
    }
}
