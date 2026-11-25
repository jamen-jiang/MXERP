using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseRequisitionOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 申购单项实体配置
/// </summary>
public sealed class PurchaseRequisitionOrderItemConfiguration : IEntityTypeConfiguration<PurchaseRequisitionOrderItem>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseRequisitionOrderItem> builder)
    {
        builder.ToTable("PurchaseRequisitionOrderItem");
        builder.Property(x => x.Purpose).IsRequired().HasMaxLength(500);
        builder.Property(x => x.DeliveryAddress).HasMaxLength(500);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.PurchaseRequisitionOrderId).HasDatabaseName("IX_PurchaseRequisitionOrderItem_PurchaseRequisitionOrderId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_PurchaseRequisitionOrderItem_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_PurchaseRequisitionOrderItem_SkuId");
    }
}
