using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Roles;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 角色表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
    }
}
