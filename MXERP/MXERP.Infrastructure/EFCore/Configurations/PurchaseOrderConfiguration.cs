using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购单实体配置
/// </summary>
public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrder");
        builder.Property(x => x.Number).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RelationType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Money).HasPrecision(18, 2);
        builder.Property(x => x.PayMode).HasMaxLength(200);
        builder.Property(x => x.DeliveryMode).HasMaxLength(200);
        builder.Property(x => x.Deliveryer).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DeliveryerTelephone).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DeliveryArea).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DeliveryAddress).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Purpose).HasMaxLength(500);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.CheckedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.PurchaseOrderItems)
            .WithOne()
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_PurchaseOrder_ProductType");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_PurchaseOrder_RelationType");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_PurchaseOrder_CompanyId");
        builder.HasIndex(x => x.SupplierId).HasDatabaseName("IX_PurchaseOrder_SupplierId");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_PurchaseOrder_Status");
    }
}
