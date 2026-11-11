using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MiniCrm.Api.ExceptionHandling;
using MiniCrm.Api.Filters;
using MiniCrm.Api.Helper;
using MiniCrm.Api.Middleware;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(o => {
    o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});

// (a) Global: áp cho mọi action
builder.Services.AddControllers(o => o.Filters.Add<LogActionFilter>()); // Add the LogActionFilter globally
builder.Services.AddScoped<CustomerExistsFilter>(); // Add the CustomerExistsFilter globally

builder.Services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
    ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier);

builder.Services.AddExceptionHandler<DomainExceptionHandler>();

builder.Services.AddHttpContextAccessor(); // accessor is singleton
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRouting(o => o.LowercaseQueryStrings = true); // Location header: /api/customers/1 thay vì /api/Customers/1

builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(); // ĐẶT ĐẦU PIPELINE, để bọc được mọi middleware phía sau
app.UseStatusCodePages(); // tuỳ chọn: 404 không khớp route cũng ra ProblemDetails

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
