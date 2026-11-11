using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniCrm.Api.Filters;

public sealed class LogActionFilter(ILogger<LogActionFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var parameters = context.ActionArguments
            .Where(kv => kv.Value is not CancellationToken)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        logger.LogInformation("Executing action: {ActionName} with parameters: {Parameters}", actionName, string.Join(", ", parameters.Keys));
        var executed = await next(); // call the next delegate to execute the action, no call = short-circuit

        if (executed.Exception is not null && !executed.ExceptionHandled)
        {
            logger.LogWarning("Action {ActionName} threw {ExceptionType}", actionName, executed.Exception.GetType().Name);
        }
        else
        {
            logger.LogInformation("Executed action: {ActionName} -> {ResultType}", actionName, executed.Result?.GetType().Name ?? "No Result");
        }
    }
}