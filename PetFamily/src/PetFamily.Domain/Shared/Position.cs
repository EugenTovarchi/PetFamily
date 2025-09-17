using CSharpFunctionalExtensions;
using Shared;

namespace PetFamily.Domain.Shared;

public class Position : ValueObject
{
    public readonly static Position First = new Position(1);
    public int Value { get; }
    private Position(int value)
    {
        Value = value;
    }

    public static Result<Position,Error> Create (int number)
    {
        if (number <= 0)
            return Errors.General.ValueMustBePositive("serial number");

        return new Position(number);    
    }

    public Result<Position, Error> Forward() => Create(Value + 1);
    public Result<Position, Error> Back() => Create(Value - 1);

    /// <summary>
    /// Сравнение обьектов будет по данным указаным внутри метода.
    /// </summary>
    /// <returns>Результат сравнения</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
