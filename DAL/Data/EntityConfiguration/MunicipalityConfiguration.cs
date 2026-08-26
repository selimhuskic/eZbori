using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class MunicipalityConfiguration : IEntityTypeConfiguration<Municipality>
    {
        public void Configure(EntityTypeBuilder<Municipality> builder)
        {
            builder.ToTable("Municipalities", "reference").HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("string").IsRequired();
            builder.Property(x => x.Canton).HasColumnType("int");
            builder.Property(x => x.Entity).HasColumnType("int").IsRequired();
            builder.Property(x => x.District).HasColumnType("bit");
            builder.Property(x => x.StateParliamentElectoralUnit).HasColumnType("int").IsRequired();
            builder.Property(x => x.EntityParliamentElectoralUnit).HasColumnType("int").IsRequired();
            builder.Property(x => x.CantonParliamentElectoralUnit).HasColumnType("int");
            builder.Property(x => x.Lattitude).HasColumnType("decimal");
            builder.Property(x => x.Longittude).HasColumnType("decimal");
            builder.Property(x => x.Population).HasColumnType("int");
            builder.Property(x => x.Area).HasColumnType("decimal");
        }
    }
}
