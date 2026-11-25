using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.EnterStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 入库单关联配置
/// </summary>
public class EnterStockOrderRelationConfiguration : IEntityTypeConfiguration<EnterStockOrderRelation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EnterStockOrderRelation> builder)
    {
        builder.ToTable("EnterStockOrderRelation");

        builder.Property(x => x.RelationType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationNumber).HasMaxLength(30).IsRequired();

        builder.HasIndex(x => x.EnterStockOrderItemId).HasDatabaseName("IX_EnterStockOrderRelation_EnterStockOrderItemId");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_EnterStockOrderRelation_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_EnterStockOrderRelation_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_EnterStockOrderRelation_RelationNumber");
    }
}
