using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Experiments;

static void SoSanh(string label, object a, object b)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"  a.Equals(b)        : {a.Equals(b)}");
    Console.WriteLine($"  ReferenceEquals    : {ReferenceEquals(a, b)}");
    Console.WriteLine($"  hash a / hash b    : {a.GetHashCode()} / {b.GetHashCode()}");
    Console.WriteLine($"  ToString(a)        : {a}");
}

var customer1 = new CustomerRecord(Guid.NewGuid(), "John Doe", "john.doe@example.com", DateTime.UtcNow, true, Contacts: [new Contact { Id = 1, Value = "contact1@example.com", Type = ContactType.Email }]);
var customer2 = new CustomerRecord(customer1.Id, "John Doe", "john.doe@example.com", customer1.CreatedAt, customer1.IsActive, Contacts: customer1.Contacts);

SoSanh("record", customer1, customer2);

Console.WriteLine($" toan tu == : {customer1 == customer2}");
Console.WriteLine($" HashSet.Count : {new HashSet<CustomerRecord> { customer1, customer2 }.Count}");

var c1 = new Customer
{
    Id = customer1.Id,
    Name = customer1.Name,
    Email = customer1.Email,
    CreatedAt = customer1.CreatedAt,
    IsActive = customer1.IsActive
};

var c2 = new Customer
{
    Id = customer2.Id,
    Name = customer2.Name,
    Email = customer2.Email,
    CreatedAt = customer2.CreatedAt,
    IsActive = customer2.IsActive
};

SoSanh("class", c1, c2);

Console.WriteLine($" toan tu == : {c1 == c2}");
Console.WriteLine($" HashSet.Count : {new HashSet<Customer> { c1, c2 }.Count}");

// with record, we can use with expression to create a new instance with modified properties
var customer3 = customer1 with { Name = "Nguyen Van A" };
customer3.Contacts.Add(new Contact { Id = 2, Value = "contact3@example.com", Type = ContactType.Email });
SoSanh("record with", customer1, customer3);

