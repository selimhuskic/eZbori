using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration;

public class ElectionCycleConfiguration : IEntityTypeConfiguration<ElectionCycle>
{
    public void Configure(EntityTypeBuilder<ElectionCycle> builder)
    {
        builder.ToTable("ElectionCycles", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.Year).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.ElectionType).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.ApiBaseUrl).HasColumnType("nvarchar(512)").IsRequired();
        builder.HasIndex(x => new { x.Year, x.ElectionType }).IsUnique();
    }
}
