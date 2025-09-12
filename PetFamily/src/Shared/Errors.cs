using Shared;
using static Errors;

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

        public static Error ValueMustBePositive(string field)
        {
            return Error.Validation("value.not.positive", $"{field} can not be negative.");
        }

        public static Error NotFoundEntity(string? field = null)
        {
            return Error.NotFound("entity.not.found", $"entity not found {field}");
        }

        public static Error ValueIsEmpty(string? field = null)
        {
            return Error.Validation("value.is.empty", $" value {field} must be not empty");
        }

        public static Error ValueIsTooLarge(string field, int maxValue)
        {
            return Error.Validation(
                "value.is.too.large",
                $"value '{field}' is too large. Max value: {maxValue}",
                invalidField: field);
        }
    }

    public static class Volunteer
    {
        public static Error CreateError(string? field = null)
        {
            return Error.Validation("entity.not.created", $"{field} has problem.");
        }
        public static Error NotFound(string? field = null)
        {
            return Error.NotFound("value.not.found", $"velue not found {field}");
        }
    }

    public static class Validation
    {
        public static Error RecordIsInvalid(string? field = null)
        {
            return Error.Validation("record.is.invalid", $"{field} is invalid.");
        }
    }

    public static class Pet
    {
        public static Error CreateError(string? field = null)
        {
            return Error.Validation("entity.not.created", $"{field} has problem.");
        }
        public static Error NotFound(string? field = null)
        {
            return Error.NotFound("value.not.found", $"velue not found {field}");
        }
        public static Error AddToVolunteer(string? field = null )
        {
            return Error.Validation("entity.not.added", $"{field} has problem.");
        }
    }

    public static class Minio
    {
        public static Error FailUpload(string? field = null)
        {
            return Error.Failure("files.not.uploaded", $"fail to  upload {field} files.");
        }
    }
}
