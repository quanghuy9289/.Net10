public class TodoServices (ITodoItemRepository repo, TimeProvider timeProvider)
{
    public async Task<TodoItem> CreateTodoItemAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }
        
        return await repo.CreateAsync(title.Trim(), timeProvider.GetUtcNow());
    }

    public async Task<TodoItem?> GetTodoItemByIdAsync(int id)
    {
        return await repo.GetByIdAsync(id);
    }
}