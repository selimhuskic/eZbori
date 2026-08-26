using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class SeededCheckConfiguration : IEntityTypeConfiguration<SeedingCheck>
    {
        public void Configure(EntityTypeBuilder<SeedingCheck> builder)
        {
            builder.ToTable("SeedingCheck", "dbo").HasKey(x => x.Id);
            builder.Property(x => x.IsSeeded).HasColumnType("bit").IsRequired();
            builder.Property(x => x.DateSeeded).HasColumnType("datetime2");
        }
    }
}