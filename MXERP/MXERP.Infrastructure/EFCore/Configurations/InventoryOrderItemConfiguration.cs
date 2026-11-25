using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.InventoryOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 盘点单详情实体配置
/// </summary>
public sealed class InventoryOrderItemConfiguration : IEntityTypeConfiguration<InventoryOrderItem>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<InventoryOrderItem> builder)
    {
        builder.ToTable("InventoryOrderItem");
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.InventoryOrderId).HasDatabaseName("IX_InventoryOrderItem_InventoryOrderId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_InventoryOrderItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_InventoryOrderItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_InventoryOrderItem_WarehouseId");
    }
}
