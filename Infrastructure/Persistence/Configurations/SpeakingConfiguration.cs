using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SpeakingConfiguration : IEntityTypeConfiguration<Speaking>
{
    public void Configure(EntityTypeBuilder<Speaking> builder)
    {
        builder.HasMany(e => e.Users).WithMany(e => e.Speakings).UsingEntity<Registration>();
    }
}
