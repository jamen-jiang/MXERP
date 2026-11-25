using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Privileges;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Privilege表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class PrivilegeConfiguration : IEntityTypeConfiguration<Privilege>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Privilege> builder)
    {
        builder.ToTable("Privilege");

        builder.Property(x => x.Master).IsRequired().HasMaxLength(50);
        builder.Property(x => x.MasterValue).IsRequired();
        builder.Property(x => x.Access).IsRequired().HasMaxLength(50);
        builder.Property(x => x.AccessValue).IsRequired();
        builder.Property(x => x.Operation).IsRequired();

        builder.HasIndex(x => x.Master).HasDatabaseName("IX_Privilege_Master");
        builder.HasIndex(x => x.MasterValue).HasDatabaseName("IX_Privilege_MasterValue");
        builder.HasIndex(x => x.Access).HasDatabaseName("IX_Privilege_Access");
        builder.HasIndex(x => x.AccessValue).HasDatabaseName("IX_Privilege_AccessValue");
    }
}
