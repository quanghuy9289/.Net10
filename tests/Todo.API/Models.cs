using System.Collections.Concurrent;

public record TodoItem(int Id, string Title, DateTimeOffset CreatedAt, bool IsDone = false);

public record CreateTodoItemRequest(string Title);

public interface ITodoItemRepository
{
    Task<TodoItem> CreateAsync(string title, DateTimeOffset createdAt);
    Task<TodoItem?> GetByIdAsync(int id);
}

public class InMemoryTodoItemRepository : ITodoItemRepository
{
    private readonly ConcurrentDictionary<int, TodoItem> _items = new();
    private int _nextId = 1;

    public Task<TodoItem> CreateAsync(string title, DateTimeOffset createdAt)
    {
        var item = new TodoItem(_nextId++, title, createdAt);
        _items[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<TodoItem?> GetByIdAsync(int id)
    {
        var item = _items.GetValueOrDefault(id);
        return Task.FromResult(item);
    }
}