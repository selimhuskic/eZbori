using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo").HasKey(x => x.Id);
        builder.Property(x => x.FirstName).HasColumnType("nvarchar").IsRequired();
        builder.Property(x => x.LastName).HasColumnType("nvarchar").IsRequired();
        builder.Property(x => x.UserName).HasColumnType("nvarchar").IsRequired();
        builder.Property(x => x.Email).HasColumnType("nvarchar").IsRequired();        
        builder.Property(x => x.Password).HasColumnType("nvarchar").IsRequired();
        builder.Property(x => x.UserRole).HasColumnType("int").IsRequired();
        builder.Property(x => x.UserVerified).HasColumnType("bit").IsRequired();
        builder.Ignore(x => x.Role);
        builder.HasOne<Application.Models.UserRole>().WithMany().HasForeignKey(x => x.UserRole);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.Property(x => x.MunicipalityId).IsRequired(false);
        builder.HasOne(x => x.MunicipalityNavigation)
            .WithMany()
            .HasForeignKey(x => x.MunicipalityId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Property(x => x.ProfileImageBase64)
            .HasColumnType("nvarchar(max)").IsRequired(false);
        builder.Property(x => x.PasswordResetToken).HasMaxLength(16).IsRequired(false);
        builder.Property(x => x.PasswordResetTokenExpiry).IsRequired(false);
    }
}
