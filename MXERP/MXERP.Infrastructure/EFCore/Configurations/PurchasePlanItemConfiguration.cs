using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchasePlans;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购计划项配置
/// </summary>
public sealed class PurchasePlanItemConfiguration : IEntityTypeConfiguration<PurchasePlanItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PurchasePlanItem> builder)
    {
        builder.ToTable("PurchasePlanItem");

        builder.Property(x => x.RelationItemId).IsRequired();
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100);
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.SkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.PurchasePlanId).HasDatabaseName("IX_PurchasePlanItem_PurchasePlanId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_PurchasePlanItem_RelationItemId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_PurchasePlanItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_PurchasePlanItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_PurchasePlanItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_PurchasePlanItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_PurchasePlanItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_PurchasePlanItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_PurchasePlanItem_WarehouseId");
    }
}
