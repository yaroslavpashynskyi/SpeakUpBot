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
                new Source
                {
                    Title = "Рекомендація від друзів",
                    Id = new Guid("4ee0db47-324f-499c-a4d4-584b5b065f7c")
                },
                new Source
                {
                    Title = "Реклама в інстаграмі",
                    Id = new Guid("94a4d26e-7684-4cd9-9bf6-63e250ac3713")
                },
                new Source
                {
                    Title = "Кафе \"Buono\"",
                    Id = new Guid("e66d6631-33d6-4d74-9ba0-e6a87658e377")
                },
                new Source
                {
                    Title = "Кафе \"Nest City Cafe\"",
                    Id = new Guid("e31133c4-578f-4abc-a678-cfcb258354a3")
                },
                new Source
                {
                    Title = "News Brovary",
                    Id = new Guid("51786e0f-ebf3-46ba-9e40-cab616a32588")
                },
                new Source
                {
                    Title = "Кафе \"І не тільки\"",
                    Id = new Guid("6c60c909-c857-4006-834b-f0a55ee0c936")
                },
                new Source
                {
                    Title = "Книжковий клуб \"Культурні\"",
                    Id = new Guid("963e4265-8d78-4107-ae64-e352aad047b6")
                },
                new Source
                {
                    Title = "Телеграм канал SpeakUp",
                    Id = new Guid("c8825caf-2504-4af3-9596-4a8392a03fc9")
                },
                new Source
                {
                    Title = "Інстраграм сторінка SpeakUp",
                    Id = new Guid("5f854f90-b8be-44b9-8a7a-7704f489fef6")
                }
            );

        base.OnModelCreating(builder);
    }
}
