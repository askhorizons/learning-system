using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities;

namespace Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken", "Identity");

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            builder.Property(p => p.Token)
                .HasColumnType("NVARCHAR(450)")
                .IsRequired();

            builder.Property(p => p.Expires)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(p => p.Revoked)
                .HasColumnType("DATETIME")
                .IsRequired(false);

            builder.HasOne(p => p.AppUser)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("FK_User_Token")
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
