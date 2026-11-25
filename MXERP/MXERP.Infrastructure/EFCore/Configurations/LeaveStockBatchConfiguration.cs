using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库批次配置
/// </summary>
public sealed class LeaveStockBatchConfiguration : IEntityTypeConfiguration<LeaveStockBatch>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockBatch> builder)
    {
        builder.ToTable("LeaveStockBatch");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PurchaseBatchNumber).HasMaxLength(30);
        builder.Property(x => x.RelationType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationId).IsRequired();
        builder.Property(x => x.RelationNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.RelationCustomerOrderNumber).HasMaxLength(100);
        builder.Property(x => x.RelationCustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.RelationOrderRef).HasMaxLength(100);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.LeaveStockQty).IsRequired();
        builder.Property(x => x.BillingQty).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsPrinted).IsRequired();
        builder.Property(x => x.PrintedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.IsThaw).IsRequired();

        builder.HasMany(x => x.LeaveStockBatchItems).WithOne().HasForeignKey(x => x.LeaveStockBatchId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.LeaveStockBatchWorkSteps).WithOne().HasForeignKey(x => x.LeaveStockBatchId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.LeaveStockBatchTrackings).WithOne().HasForeignKey(x => x.LeaveStockBatchId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Number).HasDatabaseName("IX_LeaveStockBatch_Number");
        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_LeaveStockBatch_ProductType");
        builder.HasIndex(x => x.LeaveStockPlanId).HasDatabaseName("IX_LeaveStockBatch_LeaveStockPlanId");
        builder.HasIndex(x => x.PurchaseBatchId).HasDatabaseName("IX_LeaveStockBatch_PurchaseBatchId");
        builder.HasIndex(x => x.PurchaseBatchNumber).HasDatabaseName("IX_LeaveStockBatch_PurchaseBatchNumber");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_LeaveStockBatch_RelationType");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_LeaveStockBatch_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_LeaveStockBatch_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_LeaveStockBatch_RelationNumber");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_LeaveStockBatch_DepartmentId");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_LeaveStockBatch_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_LeaveStockBatch_CompanyId");
    }
}
