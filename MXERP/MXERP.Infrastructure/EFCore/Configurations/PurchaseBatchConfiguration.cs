using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购批次实体配置
/// </summary>
public sealed class PurchaseBatchConfiguration : IEntityTypeConfiguration<PurchaseBatch>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseBatch> builder)
    {
        builder.ToTable("PurchaseBatch");
        builder.Property(x => x.Number).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PurchaseOrderNumber).HasMaxLength(200);
        builder.Property(x => x.RelationType).HasMaxLength(50);
        builder.Property(x => x.RelationNumber).HasMaxLength(30);
        builder.Property(x => x.RelationCustomerOrderNumber).HasMaxLength(100);
        builder.Property(x => x.RelationCustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.RelationOrderRef).HasMaxLength(100);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.PurchaseBatchItems)
            .WithOne()
            .HasForeignKey(x => x.PurchaseBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_PurchaseBatch_ProductType");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_PurchaseBatch_RelationType");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_PurchaseBatch_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_PurchaseBatch_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_PurchaseBatch_RelationNumber");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_PurchaseBatch_DepartmentId");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_PurchaseBatch_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_PurchaseBatch_CompanyId");
    }
}
