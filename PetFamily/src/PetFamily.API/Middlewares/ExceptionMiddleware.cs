using Shared;

namespace PetFamily.API.Middlewares;

public class ExceptionMiddleware
{
    public readonly RequestDelegate _next;
    public readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        try
        {
            _logger.LogInformation("Выполняется запрос: {Method} {Path} {QueryString}",
          request.Method,
          request.Path,
          request.QueryString);

            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Ошибка запроса: {Method} {Path} => {StatusCode}| Error: {ErrorMessage}",
            request.Method,
            request.Path,
            context.Response.StatusCode,
            ex.Message);

            var error = Error.Failure("server.internal", ex.Message);
            var envelope = Envelope.Error(error); //убрал ([error]) - ругался компилятор и требовал конструктор 

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(envelope);
        }
    }
}

/// <summary>
/// Метод расширения для использования ExceptionMiddleware в Program.cs
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
