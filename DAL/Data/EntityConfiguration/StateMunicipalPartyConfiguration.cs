using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    internal class StateMunicipalPartyConfiguration : IEntityTypeConfiguration<StateMunicipalParty>
    {
        public void Configure(EntityTypeBuilder<StateMunicipalParty> builder)
        {
            builder.ToTable("StateMunicipalParties", "elections").HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectoralUnitPartyResultId).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.Name).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.HasOne<Municipality>().WithMany().HasForeignKey(x => x.MunicipalityCode);
        }
    }
}
