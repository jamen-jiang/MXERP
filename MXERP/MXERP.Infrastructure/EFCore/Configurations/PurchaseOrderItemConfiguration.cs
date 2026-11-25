using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购单项实体配置
/// </summary>
public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItem");
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.TaxRate).HasPrecision(18, 2);
        builder.Property(x => x.Money).HasPrecision(18, 2);
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasMany(x => x.PurchaseOrderRelations)
            .WithOne()
            .HasForeignKey(x => x.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PurchaseOrderId).HasDatabaseName("IX_PurchaseOrderItem_PurchaseOrderId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_PurchaseOrderItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_PurchaseOrderItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_PurchaseOrderItem_WarehouseId");
    }
}
