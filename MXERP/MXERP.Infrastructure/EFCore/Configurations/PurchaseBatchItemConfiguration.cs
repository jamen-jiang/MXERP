using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购批次项实体配置
/// </summary>
public sealed class PurchaseBatchItemConfiguration : IEntityTypeConfiguration<PurchaseBatchItem>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseBatchItem> builder)
    {
        builder.ToTable("PurchaseBatchItem");
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100);
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.PurchaseBatchId).HasDatabaseName("IX_PurchaseBatchItem_PurchaseBatchId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_PurchaseBatchItem_RelationItemId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_PurchaseBatchItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_PurchaseBatchItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_PurchaseBatchItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_PurchaseBatchItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_PurchaseBatchItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_PurchaseBatchItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_PurchaseBatchItem_WarehouseId");
    }
}
