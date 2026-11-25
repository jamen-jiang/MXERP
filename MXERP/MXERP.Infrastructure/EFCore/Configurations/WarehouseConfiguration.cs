using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Warehouses;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 仓库表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouse");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.ManageManName).HasMaxLength(20);
        builder.Property(x => x.Telephone).HasMaxLength(20);
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Warehouse_Name");
        builder.HasIndex(x => x.IsDefault).HasDatabaseName("IX_Warehouse_IsDefault");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Warehouse_Code").HasFilter("[IsDeleted] = 0");
    }
}
