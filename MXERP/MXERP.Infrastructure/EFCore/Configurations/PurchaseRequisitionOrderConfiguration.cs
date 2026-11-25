using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseRequisitionOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 申购单实体配置
/// </summary>
public sealed class PurchaseRequisitionOrderConfiguration : IEntityTypeConfiguration<PurchaseRequisitionOrder>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseRequisitionOrder> builder)
    {
        builder.ToTable("PurchaseRequisitionOrder");
        builder.Property(x => x.Number).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.CommittedByName).HasMaxLength(50);
        builder.Property(x => x.CheckedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.PurchaseRequisitionOrderItems)
            .WithOne()
            .HasForeignKey(x => x.PurchaseRequisitionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_PurchaseRequisitionOrder_DepartmentId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_PurchaseRequisitionOrder_CompanyId");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_PurchaseRequisitionOrder_Status");
    }
}
