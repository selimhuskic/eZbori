using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class CantonMunicipalOverviewConfiguration : IEntityTypeConfiguration<CantonMunicipalOverview>
    {
        public void Configure(EntityTypeBuilder<CantonMunicipalOverview> builder)
        {
            builder.ToTable("CantonMunicipalOverview", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.CantonCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
        }
    }
}
