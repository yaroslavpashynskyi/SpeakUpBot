using System.Reflection;
using System.Reflection.Emit;

using Application.Common.Interfaces;

using Domain.Entities;
using Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Registration> Registrations => Set<Registration>();

    public DbSet<Source> Sources => Set<Source>();

    public DbSet<Speaking> Speakings => Set<Speaking>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Venue> Venues => Set<Venue>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.HasPostgresEnum<EnglishLevel>();
        builder.HasPostgresEnum<PaymentStatus>();
        builder.HasPostgresEnum<Role>();

        builder
            .Entity<Source>()
            .HasData(
                new Source { Title = "Рекомендація від друзів", Id = Guid.NewGuid() },
                new Source { Title = "Реклама в інстаграмі", Id = Guid.NewGuid() },
                new Source { Title = "Кафе \"Buono\"", Id = Guid.NewGuid() },
                new Source { Title = "Кафе \"Nest City Cafe\"", Id = Guid.NewGuid() },
                new Source { Title = "News Brovary", Id = Guid.NewGuid() },
                new Source { Title = "Кафе \"І не тільки\"", Id = Guid.NewGuid() },
                new Source { Title = "Книжковий клуб \"Культурні\"", Id = Guid.NewGuid() },
                new Source { Title = "Телеграм канал SpeakUp", Id = Guid.NewGuid() },
                new Source { Title = "Інстраграм сторінка SpeakUp", Id = Guid.NewGuid() }
            );

        base.OnModelCreating(builder);
    }
}
