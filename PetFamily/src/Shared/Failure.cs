using System.Collections;

namespace Shared;

/// <summary>
/// Экземпляры Failure можно перебирать в цикле foreach и использовать LINQ, и другие методы работы с коллекциями
/// </summary>
public class Failure : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    //Принимает IEnumerable<Error> (любую коллекцию ошибок)
    public Failure(IEnumerable<Error> errors)
    {
        _errors = [.. errors]; //создаёт новый список, копируя элементы из входной коллекции
    }

    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //Когда передается список ошибок - запускается конструктор,который копирует их в массив.
    public static implicit operator Failure(Error[] errors) => new Failure(errors);

    //Когда хотим вернуть одну ошибку - делает список из 1й ошибки.
    public static implicit operator Failure(Error error) => new Failure([error]);
}

/*этот класс представляет собой неизменяемую (immutable) обёртку вокруг списка ошибок,
 * которую можно использовать как коллекцию.
 * Это удобно для возврата множества ошибок из методов или сервисов.
 */

