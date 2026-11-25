using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.AuditLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// AuditLog表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");

        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.EntityType).HasDatabaseName("IX_AuditLog_EntityType");
        builder.HasIndex(x => x.EntityId).HasDatabaseName("IX_AuditLog_EntityId");
        builder.HasIndex(x => x.Action).HasDatabaseName("IX_AuditLog_Action");
    }
}
