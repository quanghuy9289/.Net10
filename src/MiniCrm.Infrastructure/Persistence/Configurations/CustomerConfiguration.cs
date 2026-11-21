using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(320);
        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.CreatedAt);

        // Configure the relationship between Customer and Contact 
        // 1-to-many that don't need navigational properties in the entities
        builder.HasMany(c => c.Contacts)
            .WithOne()
            .HasForeignKey(n => n.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Note>()
            .WithOne()
            .HasForeignKey(n => n.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // seed data
        builder.HasData(
            new Customer
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Acme Corp",
                Email = "contact@acme.test",
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2026, 11, 21, 10, 41, 49, 905, DateTimeKind.Unspecified).AddTicks(9413), new TimeSpan(0, 0, 0, 0, 0))
            }
        );
    }
}