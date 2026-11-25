using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Users;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// User_Role表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("User_Role");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RoleId).IsRequired();

        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_User_Role_UserId");
        builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_User_Role_RoleId");
    }
}
