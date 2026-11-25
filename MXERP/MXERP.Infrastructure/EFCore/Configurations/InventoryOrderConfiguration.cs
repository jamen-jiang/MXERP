using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.InventoryOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 盘点单实体配置
/// </summary>
public sealed class InventoryOrderConfiguration : IEntityTypeConfiguration<InventoryOrder>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<InventoryOrder> builder)
    {
        builder.ToTable("InventoryOrder");
        builder.Property(x => x.Number).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasMany(x => x.InventoryOrderItems)
            .WithOne()
            .HasForeignKey(x => x.InventoryOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_InventoryOrder_ProductType");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_InventoryOrder_Status");
    }
}
