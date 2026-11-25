using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockPlans;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库计划项配置
/// </summary>
public sealed class LeaveStockPlanItemConfiguration : IEntityTypeConfiguration<LeaveStockPlanItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockPlanItem> builder)
    {
        builder.ToTable("LeaveStockPlanItem");

        builder.Property(x => x.RelationItemId).IsRequired();
        builder.Property(x => x.SalesSpuCode).HasMaxLength(100);
        builder.Property(x => x.CustomerSpuCode).HasMaxLength(100);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.SkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.LeaveStockPlanId).HasDatabaseName("IX_LeaveStockPlanItem_LeaveStockPlanId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_LeaveStockPlanItem_RelationItemId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_LeaveStockPlanItem_SalesSpuId");
        builder.HasIndex(x => x.SalesSpuCode).HasDatabaseName("IX_LeaveStockPlanItem_SalesSpuCode");
        builder.HasIndex(x => x.CustomerSpuId).HasDatabaseName("IX_LeaveStockPlanItem_CustomerSpuId");
        builder.HasIndex(x => x.CustomerSpuCode).HasDatabaseName("IX_LeaveStockPlanItem_CustomerSpuCode");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_LeaveStockPlanItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_LeaveStockPlanItem_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_LeaveStockPlanItem_WarehouseId");
    }
}
