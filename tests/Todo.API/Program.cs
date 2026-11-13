var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ITodoItemRepository, InMemoryTodoItemRepository>();
builder.Services.AddScoped<TodoServices>();

var app = builder.Build();

app.MapGet("/todos/{id:int}", TodoEndpoints.GetTodoItemById);
app.MapPost("/todos", TodoEndpoints.CreateTodoItem);

app.Run();