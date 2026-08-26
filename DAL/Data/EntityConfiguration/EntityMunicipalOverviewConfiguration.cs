using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class EntityMunicipalOverviewConfiguration : IEntityTypeConfiguration<EntityMunicipalOverview>
    {
        public void Configure(EntityTypeBuilder<EntityMunicipalOverview> builder)
        {
            builder.ToTable("EntityMunicipalOverview", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.NumberOfParties).HasColumnType("int").IsRequired();
            builder.Property(x => x.NumberOfCandidates).HasColumnType("int").IsRequired();
            builder.Property(x => x.InvalidBlankBallots).HasColumnType("int").IsRequired();
            builder.Property(x => x.InvalidOthersBallots).HasColumnType("int").IsRequired();
            builder.Property(x => x.MunicipalityCode).HasColumnType("int").IsRequired();
            builder.Property(x => x.PercentageProcessedPollingStations).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ElectoralUnitId).HasColumnType("int").IsRequired();
            builder.Property(x => x.NumberOfVoters).HasColumnType("int").IsRequired();
            builder.Property(x => x.PercentageTotalNoVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.PercentageTotalVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedInvalidBlankBallots).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedInvalidOthersBallots).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedPollingStations).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedValidVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.ProcessedTotalInvalidVotes).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.TotalNoVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalInvalidVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalPollingStations).HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.Property(x => x.ValidVotes).HasColumnType("int").IsRequired();
        }
    }
}
