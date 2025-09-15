using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared;

/// <summary>
/// Класс расширения ответов для Controller
/// </summary>
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
            StatusCode = statusCode,
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
}
