using MiniCrm.Domain.Experiments;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Tests.Experiments;

public class CustomerRecordTest
{
    [Fact]
    public void RecordsWithTheSameListValues_AreNotEqual()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var first = new CustomerRecord(id, "Alex Smith", "alex@example.com", createdAt, true, Contacts: []);
        var second = new CustomerRecord(id, "Alex Smith", "alex@example.com", createdAt, true, Contacts: []);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void RecordsWithTheSameValues_AreEqual()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        List<Contact> contacts = [new Contact { Id = Guid.NewGuid(), CustomerId = id, Type = ContactType.Email, Value = "alex@example.com" }];

        var first = new CustomerRecord(id, "Alex Smith", "alex@example.com", createdAt, true, contacts);
        var second = new CustomerRecord(id, "Alex Smith", "alex@example.com", createdAt, true, contacts);

        Assert.Equal(first, second);
    }
}