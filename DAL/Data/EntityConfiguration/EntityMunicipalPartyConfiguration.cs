using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class EntityMunicipalPartyConfiguration : IEntityTypeConfiguration<EntityMunicipalParty>
    {
        public void Configure(EntityTypeBuilder<EntityMunicipalParty> builder)
        {
            builder.ToTable("EntityMunicipalParty", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.ElectoralUnitPartyResultId).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.Name).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.HasOne<Municipality>().WithMany().HasForeignKey(x => x.MunicipalityCode);
        }
    }
}