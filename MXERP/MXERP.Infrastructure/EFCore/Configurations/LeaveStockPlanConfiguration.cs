using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockPlans;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库计划配置
/// </summary>
public sealed class LeaveStockPlanConfiguration : IEntityTypeConfiguration<LeaveStockPlan>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockPlan> builder)
    {
        builder.ToTable("LeaveStockPlan");

        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationId).IsRequired();
        builder.Property(x => x.RelationNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.RelationCustomerOrderNumber).HasMaxLength(100);
        builder.Property(x => x.RelationCustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.RelationOrderRef).HasMaxLength(100);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasMany(x => x.LeaveStockPlanItems).WithOne().HasForeignKey(x => x.LeaveStockPlanId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_LeaveStockPlan_ProductType");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_LeaveStockPlan_RelationType");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_LeaveStockPlan_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_LeaveStockPlan_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_LeaveStockPlan_RelationNumber");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_LeaveStockPlan_DepartmentId");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_LeaveStockPlan_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_LeaveStockPlan_CompanyId");
    }
}
