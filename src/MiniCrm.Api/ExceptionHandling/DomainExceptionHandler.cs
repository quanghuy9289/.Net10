using Microsoft.AspNetCore.Diagnostics;
using MiniCrm.Domain.Exceptions;

namespace MiniCrm.Api.ExceptionHandling;

public sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false; // Not a domain exception, let other handlers handle it, default 500
        }

        var (status, title) = domainException switch
        {
            CustomerNotFoundException => (StatusCodes.Status404NotFound, "Customer Not Found"),
            DuplicateEmailException => (StatusCodes.Status409Conflict, "Duplicate Email"),
            _ => (StatusCodes.Status500InternalServerError, "Domain rule violation")
        };

        logger.LogWarning(exception, "Domain exception mapped to {StatusCode}", status);

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new()
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            }
        });
    }

}