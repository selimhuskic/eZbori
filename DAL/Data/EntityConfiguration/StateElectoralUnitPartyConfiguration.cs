using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class StateElectoralUnitPartyConfiguration : IEntityTypeConfiguration<StateElectoralUnitParty>
    {
        public void Configure(EntityTypeBuilder<StateElectoralUnitParty> builder)
        {
            builder.ToTable("StateElectoralUnitParty", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.AbsenceAndMobileTeamVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.CompensationMandates).HasColumnType("int").IsRequired();
            builder.Property(x => x.ConfirmedVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectoralUnitParentPartyResultId).HasColumnType("int").IsRequired();
            builder.Property(x => x.PartyName).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.PostOfficeVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.RegularMandates).HasColumnType("int").IsRequired();
            builder.Property(x => x.RegularVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
        }
    }
}
