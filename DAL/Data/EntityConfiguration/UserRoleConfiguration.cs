using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserRole = Application.Models.UserRole;

namespace DAL.Data.EntityConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.RoleName).HasColumnType("nvarchar").IsRequired();
        builder.HasIndex(x => x.RoleName).IsUnique();
    }
}
