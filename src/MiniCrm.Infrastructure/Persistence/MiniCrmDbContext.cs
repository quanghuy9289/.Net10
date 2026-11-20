using Microsoft.EntityFrameworkCore;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Infrastructure.Persistence;

public sealed class MiniCrmDbContext(DbContextOptions<MiniCrmDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniCrmDbContext).Assembly);
}
