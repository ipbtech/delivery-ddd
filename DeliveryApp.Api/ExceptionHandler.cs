using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Api;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode;
        switch (exception)
        {
            case NullReferenceException:
                statusCode = StatusCodes.Status404NotFound; 
                break;
            case ArgumentException:
                statusCode = StatusCodes.Status400BadRequest;
                break;
            case DbUpdateException:
                statusCode = StatusCodes.Status400BadRequest;
                break;
            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = statusCode,
            Detail = exception.Message
        };

        logger.LogError(exception, "An error {@ErrorType} occured with details: {@Details}", exception.GetType(), exception.Message);
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}