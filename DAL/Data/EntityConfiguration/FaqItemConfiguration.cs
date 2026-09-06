using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration;

public class FaqItemConfiguration : IEntityTypeConfiguration<FaqItem>
{
    public void Configure(EntityTypeBuilder<FaqItem> builder)
    {
        builder.ToTable("FaqItems", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.Question).HasColumnType("nvarchar(500)").IsRequired();
        builder.Property(x => x.Answer).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnType("int").IsRequired();
    }
}
