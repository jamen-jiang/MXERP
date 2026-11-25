using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PurchaseOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 采购单关联表实体配置
/// </summary>
public sealed class PurchaseOrderRelationConfiguration : IEntityTypeConfiguration<PurchaseOrderRelation>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<PurchaseOrderRelation> builder)
    {
        builder.ToTable("PurchaseOrderRelation");
        builder.Property(x => x.RelationType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RelationNumber).IsRequired().HasMaxLength(30);

        builder.HasIndex(x => x.PurchaseOrderItemId).HasDatabaseName("IX_PurchaseOrderRelation_PurchaseOrderItemId");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_PurchaseOrderRelation_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_PurchaseOrderRelation_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_PurchaseOrderRelation_RelationNumber");
    }
}
