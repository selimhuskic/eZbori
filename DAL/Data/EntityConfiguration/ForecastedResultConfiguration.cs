using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration;

public class ForecastedResultConfiguration : IEntityTypeConfiguration<ForecastedResult>
{
    public void Configure(EntityTypeBuilder<ForecastedResult> builder)
    {
        builder.ToTable("ForecastedResults", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.MunicipalCode).HasColumnType("smallint");
        builder.Property(x => x.CantonCode).HasColumnType("smallint");
        builder.Property(x => x.EntityCode).HasColumnType("smallint");
        builder.Property(x => x.IsStateCouncil).HasColumnType("bit").IsRequired();
        builder.Property(x => x.ForecastedNumberOfVotes).HasColumnType("float");
        builder.Property(x => x.PartyName).HasColumnType("nvarchar(512)").IsRequired();
        builder.Property(x => x.ElectionYear).HasColumnType("smallint");
    }
}
