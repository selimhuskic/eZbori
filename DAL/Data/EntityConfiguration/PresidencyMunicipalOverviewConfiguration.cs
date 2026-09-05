using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class PresidencyMunicipalOverviewConfiguration : IEntityTypeConfiguration<PresidencyMunicipalOverview>
    {
        public void Configure(EntityTypeBuilder<PresidencyMunicipalOverview> builder)
        {
            builder.ToTable("PresidencyMunicipalOverview", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.Entity).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVoters).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalNoVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ValidVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalInvalidVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.InvalidBlankBallots).HasColumnType("int").IsRequired();
            builder.Property(x => x.InvalidOthersBallots).HasColumnType("int").IsRequired();
            builder.Property(x => x.ProcessedPollingStationsPercentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.PercentageTotalVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.PercentageTotalNoVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedTotalInvalidVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedInvalidBlankBallots).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedInvalidOthersBallots).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.TotalPollingStations).HasColumnType("int").IsRequired();
            builder.Property(x => x.ProcessedPollingStations).HasColumnType("int").IsRequired();
            builder.Property(x => x.PartyNumber).HasColumnType("int").IsRequired();
            builder.Property(x => x.CandidatesNumber).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
            builder.HasOne<Municipality>().WithMany().HasForeignKey(x => x.MunicipalityCode);
        }
    }
}
