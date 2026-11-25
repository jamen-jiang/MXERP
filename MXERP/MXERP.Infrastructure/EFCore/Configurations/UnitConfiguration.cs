using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Units;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 单位表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Unit");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Unit_Name");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_Unit_Sort");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Unit_Code").HasFilter("[IsDeleted] = 0");
    }
}
