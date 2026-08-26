using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class CantonMunicipalPartyConfiguration : IEntityTypeConfiguration<CantonMunicipalParty>
    {
        public void Configure(EntityTypeBuilder<CantonMunicipalParty> builder)
        {
            builder.ToTable("CantonMunicipalParties", "elections").HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.CantonCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
        }
    }
}
