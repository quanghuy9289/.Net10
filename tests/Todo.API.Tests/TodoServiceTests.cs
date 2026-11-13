using Microsoft.Extensions.Time.Testing;

public class TodoServiceTests
{
    [Fact] //single test case
    public async Task CreateAsync_TrimsTitle_AndUsesCurrentTime()
    {
        // Arrange
        var now = new DateTimeOffset(2026,1,1,8,0,0,TimeSpan.Zero);
        var service = new TodoServices(new InMemoryTodoItemRepository(), new FakeTimeProvider(now));
        
        var title = "  Test Todo Item  ";
        var expectedTitle = title.Trim();
        var expectedCreatedAt = now;

        // Act
        var todoItem = await service.CreateTodoItemAsync(title);

        // Assert
        Assert.Equal(expectedTitle, todoItem.Title);
        Assert.Equal(expectedCreatedAt, todoItem.CreatedAt);
    }

    [Theory] // multiple test cases
    [InlineData("")] // input parameter: empty string
    [InlineData("   ")] // input parameter: whitespace string
    public async Task CreateAsync_EmptyTitle_Throws(string title)
    {
        // Arrange
        var service = new TodoServices(new InMemoryTodoItemRepository(), TimeProvider.System);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTodoItemAsync(title));
    }
}