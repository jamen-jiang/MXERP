using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchasePlans;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购计划配置
/// </summary>
public sealed class PurchasePlanConfiguration : IEntityTypeConfiguration<PurchasePlan>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PurchasePlan> builder)
    {
        builder.ToTable("PurchasePlan");

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

        builder.HasMany(x => x.PurchasePlanItems).WithOne().HasForeignKey(x => x.PurchasePlanId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_PurchasePlan_ProductType");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_PurchasePlan_RelationType");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_PurchasePlan_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_PurchasePlan_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_PurchasePlan_RelationNumber");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_PurchasePlan_DepartmentId");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_PurchasePlan_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_PurchasePlan_CompanyId");
    }
}
