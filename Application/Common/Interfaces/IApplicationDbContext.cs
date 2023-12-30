using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Registration> Registrations { get; }
    public DbSet<Source> Sources { get; }
    public DbSet<Speaking> Speakings { get; }
    public DbSet<User> Users { get; }
    public DbSet<Venue> Venues { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
