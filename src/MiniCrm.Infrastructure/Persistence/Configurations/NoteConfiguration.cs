using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

internal sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.CustomerId);
        builder.Property(n => n.Content).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.AuthorId).IsRequired().HasMaxLength(100);
        builder.Property(n => n.CreatedAt);
    }
}