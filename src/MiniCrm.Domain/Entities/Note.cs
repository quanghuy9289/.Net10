namespace MiniCrm.Domain.Entities;

public sealed class Note
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Content { get; init; } = string.Empty;
    public string AuthorId { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
