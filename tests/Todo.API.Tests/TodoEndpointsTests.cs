using Microsoft.AspNetCore.Http.HttpResults;

public class TodoEndpointsTests
{
    [Fact]
    public async Task GetTodoItemById_NotExisting_ReturnsNotFound()
    {
        // Arrange
        var service = new TodoServices(new InMemoryTodoItemRepository(), TimeProvider.System);

        // Act
        var result = await TodoEndpoints.GetTodoItemById(999, service);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task CreateTodoItem_EmptyTitle_ReturnsValidationProblem()
    {
        // Arrange
        var service = new TodoServices(new InMemoryTodoItemRepository(), TimeProvider.System);
        var request = new CreateTodoItemRequest("");

        // Act
        var result = await TodoEndpoints.CreateTodoItem(request, service);

        // Assert
        Assert.IsType<ValidationProblem>(result.Result);
    }
}