using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

internal sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.CustomerId);
        builder.Property(c => c.Value).HasMaxLength(200);

        // seed data
        builder.HasData(
            new Contact
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Type = ContactType.Email,
                Value = "contact@acme.test"
            }
        );
    }
}