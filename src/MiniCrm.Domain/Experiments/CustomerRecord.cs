using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Experiments;

public sealed record CustomerRecord(Guid Id, string Name, string Email, DateTime CreatedAt, bool IsActive, List<Contact> Contacts);
