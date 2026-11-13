using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

public class TodoApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Post_ThenGet_ReturnsSameTodo()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var client = factory.CreateClient();

        // create a new todo item
        var response = await client.PostAsJsonAsync("/todos",
            new CreateTodoItemRequest("Learn Testing"), ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // read the todo item back
        var location = new Uri(client.BaseAddress!, response.Headers.Location!);
        var todo = await client.GetFromJsonAsync<TodoItem>(location.ToString(), ct);
        Assert.Equal("Learn Testing", todo!.Title);
    }

    [Fact]
    public async Task Post_EmptyTitle_Returns400()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/todos", new CreateTodoItemRequest(""),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}