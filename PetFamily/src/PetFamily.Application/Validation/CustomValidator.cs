using FluentValidation;
using Shared;

namespace PetFamily.Application.Validation;

public static class CustomValidator
{
    public static IRuleBuilderOptionsConditions<T,TElement> MustBeValueObject<T,TElement,TValueObject>
        (this IRuleBuilder<T,TElement> ruleBuilder, Func<TElement, Result<TValueObject>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject> result = factoryMethod(value);

            if(!result.IsSuccess)
            {
                context.AddFailure(result.Error.Serialize());
            }
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
     this IRuleBuilderOptions<T,TProperty> rule,
     string errorCode,
     string errorMessage)
    {
        var error = Error.Validation(errorCode, errorMessage);
        return rule.WithMessage(error.Serialize());
    }
}
