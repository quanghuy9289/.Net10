namespace MiniCrm.Domain.Entities;

public sealed class Note
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
