using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class MunicipalityCandidateDetailsConfiguration : IEntityTypeConfiguration<MunicipalityCandidateDetails>
    {

        public void Configure(EntityTypeBuilder<MunicipalityCandidateDetails> builder)
        {
            builder.ToTable("MunicipalityCandidateDetails", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.Name).HasColumnType("nvarchar(512)").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar(512)").IsRequired();
            builder.Property(x => x.HaveMandates).HasColumnType("bit").IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.AbsenceAndMobileTeamVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ConfirmedVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.PostOfficeVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.RegularVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.HasOne<Municipality>().WithMany().HasForeignKey(x => x.MunicipalityCode);
        }
    }
}