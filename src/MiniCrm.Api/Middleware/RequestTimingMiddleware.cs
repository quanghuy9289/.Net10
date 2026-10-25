using System.Diagnostics;

namespace MiniCrm.Api.Middleware;
public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();
        try
        {
            await next(context);
            //throw new InvalidOperationException("This is a test exception in middleware");
        }
        finally
        {
            //context.Response.Headers["X-Request-Duration-ms"] = elapsed.TotalMilliseconds.ToString("0.0");
            // context.Response.OnStarting(() =>
            // {
                
            //     context.Response.Headers["X-Request-Duration-ms"] = elapsed.TotalMilliseconds.ToString("0.0");
            //     return Task.CompletedTask;
            // });
            var elapsed = Stopwatch.GetElapsedTime(start);
            logger.LogInformation("Request {Method} {Path} -> {StatusCode} executed in {ElapsedMs:0.0} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsed.TotalMilliseconds);
        }
    }
}