using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Failure failure)
    {
        if (failure == null || !failure.Any())
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        var distinctErrorTypes = failure
            .Select(x => x.Type)
            .Distinct()
            .ToList();

        if (distinctErrorTypes.Count == 0)
        {
            return new ObjectResult(failure)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeFromErrorType((ErrorType)distinctErrorTypes.First()!);

        return new ObjectResult(failure)
        {
            StatusCode = statusCode
        };
    }
    private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
       errorType switch
       {
           ErrorType.Validation => StatusCodes.Status400BadRequest,
           ErrorType.NotFound => StatusCodes.Status404NotFound,
           ErrorType.Conflict => StatusCodes.Status409Conflict,
           ErrorType.Failure => StatusCodes.Status500InternalServerError,
           _ => StatusCodes.Status500InternalServerError
       };

    public static ActionResult ToResponse(this Error error)
    {
        return error.ToFailure().ToResponse(); // Преобразуем в Failure и вызываем основной метод
    }

    //public static ActionResult<T> ToResponse<T>(this Result<T, Error> result)  //под CSFunc бибилиотеку
    //{
    //    if (result.IsSuccess)
    //    {
    //        return new OkObjectResult(Envelope.Ok(result.Value));
    //    }
    //    var statusCode = result.Type switch
    //    {
    //        ErrorType.Validation => StatusCodes.Status400BadRequest,
    //        ErrorType.NotFound => StatusCodes.Status404NotFound,
    //        ErrorType.Conflict => StatusCodes.Status409Conflict,
    //        ErrorType.Failure => StatusCodes.Status500InternalServerError,
    //        _ => StatusCodes.Status500InternalServerError
    //    };
    //    var envelope = Envelope.Error(result.Error);

    //    return new ObjectResult(envelope)
    //    {
    //        StatusCode = statusCode
    //    };
    //}
}
