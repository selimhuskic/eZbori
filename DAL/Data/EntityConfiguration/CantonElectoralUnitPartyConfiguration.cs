using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class CantonElectoralUnitPartyConfiguration : IEntityTypeConfiguration<CantonElectoralUnitParty>
    {
        public void Configure(EntityTypeBuilder<CantonElectoralUnitParty> builder)
        {
            builder.ToTable("CantonElectoralUnitParty", "elections").HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.CantonElectoralUnitCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.AbsenceAndMobileTeamVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasMaxLength(512).IsRequired();
            builder.Property(x => x.ConfirmedVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectoralUnitParentPartyResultId).HasColumnType("int").IsRequired();
            builder.Property(x => x.Mandates).HasColumnType("int").IsRequired();
            builder.Property(x => x.Name).HasMaxLength(512).IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.PostOfficeVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.RegularVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
        }
    }
}
