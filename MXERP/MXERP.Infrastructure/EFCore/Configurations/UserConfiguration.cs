using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Users;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// User表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Password).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_User_Name");
        builder.HasIndex(x => x.UserName).IsUnique().HasDatabaseName("UX_User_UserName").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.UserPosts)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UserRoles)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
