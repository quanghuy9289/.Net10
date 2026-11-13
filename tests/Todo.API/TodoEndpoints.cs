using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;

public static class TodoEndpoints
{
    public static async Task<Results<Created<TodoItem>, ValidationProblem>> CreateTodoItem(
        CreateTodoItemRequest request,
        TodoServices todoServices)
    {
        try
        {
            var todoItem = await todoServices.CreateTodoItemAsync(request.Title);
            return TypedResults.Created($"/todos/{todoItem.Id}", todoItem);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { ex.ParamName ?? "Title", new[] { ex.Message } }
            });
        }
    }

    public static async Task<Results<Ok<TodoItem>, NotFound>> GetTodoItemById(
        int id,
        TodoServices todoServices)
    {
        var todoItem = await todoServices.GetTodoItemByIdAsync(id);
        if (todoItem is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(todoItem);
    }
}