using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class EntityPresidentMunicipalCandidateConfiguration : IEntityTypeConfiguration<EntityPresidentMunicipalCandidate>
    {
        public void Configure(EntityTypeBuilder<EntityPresidentMunicipalCandidate> builder)
        {
            builder.ToTable("EntityPresidentMunicipalCandidate", "elections").HasKey(x => x.Id);
            builder.Property(x => x.ElectionYear).HasColumnType("int").IsRequired();
            builder.Property(x => x.Code).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.Name).HasColumnType("nvarchar").IsRequired();
            builder.Property(x => x.Percentage).HasColumnType("decimal").IsRequired();
            builder.Property(x => x.TotalVotes).HasColumnType("int").IsRequired();
            builder.HasOne<Municipality>().WithMany().HasForeignKey(x => x.MunicipalityCode);
        }
    }
}
