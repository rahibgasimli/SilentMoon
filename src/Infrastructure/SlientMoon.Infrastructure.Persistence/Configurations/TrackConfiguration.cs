using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasOne(t => t.Narrator)
                   .WithMany()
                   .HasForeignKey(t => t.NarratorId)
                   .IsRequired();
        }
    }
}