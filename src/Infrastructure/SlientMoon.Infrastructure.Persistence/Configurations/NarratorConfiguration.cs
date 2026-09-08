using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class NarratorConfiguration : IEntityTypeConfiguration<Narrator>
    {
        public void Configure(EntityTypeBuilder<Narrator> builder)
        {
            builder.ToTable("Narrators");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Name).IsRequired().HasMaxLength(150);
        }
    }
}
