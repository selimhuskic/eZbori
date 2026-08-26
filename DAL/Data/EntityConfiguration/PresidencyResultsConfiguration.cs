using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class PresidencyResultsConfiguration : IEntityTypeConfiguration<PresidencyResults>
    {
        public void Configure(EntityTypeBuilder<PresidencyResults> builder)
        {
            builder.ToTable("PresidencyResults", "elections").HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("int").IsRequired();
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.CandidateName).HasColumnType("string").IsRequired();
            builder.Property(x => x.Constituency).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ConfirmedVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.AbsenceAndMobileTeamVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.RegularVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.MandateWon).HasColumnType("bit").IsRequired();
        }
    }
}
