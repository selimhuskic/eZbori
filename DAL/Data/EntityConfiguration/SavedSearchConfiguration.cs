using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration;

public class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
{
    public void Configure(EntityTypeBuilder<SavedSearch> builder)
    {
        builder.ToTable("SavedSearches", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnType("int").IsRequired();
        builder.Property(x => x.ElectionType).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.ElectionYear).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.AnalysisSubject).HasColumnType("tinyint");
        builder.Property(x => x.ElectoralUnit).HasColumnType("int");
        builder.Property(x => x.MunicipalityCode).HasColumnType("int");
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnType("bit").IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Municipality>()
            .WithMany()
            .HasForeignKey(x => x.MunicipalityCode);
    }
}
