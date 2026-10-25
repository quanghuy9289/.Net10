using MiniCrm.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// inline middleware to log the request and response for debugging purposes
app.Use(async (context, next) =>
{
    // Log the request
    app.Logger.LogInformation("A -> before | Endpoint: {Endpoint}", context.GetEndpoint()?.DisplayName ?? "No endpoint");
    // Call the next middleware in the pipeline
    await next(context);

    // Log the response
    app.Logger.LogInformation("A -> after | Status Code: {StatusCode}", context.Response.StatusCode);
});

app.UseRouting(); // call explicitly to ensure that the endpoint is set before the next middleware runs

app.UseRequestTiming(); // add the request timing middleware to the pipeline

//app.Run(async ctx => await ctx.Response.WriteAsync("Cụt")); // terminal middleware to handle requests that don't match any route

app.Use(async (context, next) =>
{
    // Log the request
    app.Logger.LogInformation("B -> before | Endpoint: {Endpoint}", context.GetEndpoint()?.DisplayName ?? "No endpoint");
    //await context.Response.WriteAsync("Hi");
    // Call the next middleware in the pipeline
    await next(context);

    // Log the response
    app.Logger.LogInformation("B -> after | Status Code: {StatusCode}", context.Response.StatusCode);
});

app.Use(async (context, next) =>
{
    // Log the request
    app.Logger.LogInformation("C -> before | Endpoint: {Endpoint}", context.GetEndpoint()?.DisplayName ?? "No endpoint");
    // Call the next middleware in the pipeline
    await next(context);

    // Log the response
    app.Logger.LogInformation("C -> after | Status Code: {StatusCode}", context.Response.StatusCode);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
