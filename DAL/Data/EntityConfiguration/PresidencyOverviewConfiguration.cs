using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration
{
    public class PresidencyOverviewConfiguration : IEntityTypeConfiguration<PresidencyOverview>
    {
        public void Configure(EntityTypeBuilder<PresidencyOverview> builder)
        {
            builder.ToTable("PresidencyOverview", "elections").HasKey(x => x.Id);
        }
    }
}
