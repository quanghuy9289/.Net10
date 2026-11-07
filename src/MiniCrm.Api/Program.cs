using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MiniCrm.Api.Helper;
using MiniCrm.Api.Middleware;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(o => {
    o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
