using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库单关联配置
/// </summary>
public class LeaveStockOrderRelationConfiguration : IEntityTypeConfiguration<LeaveStockOrderRelation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockOrderRelation> builder)
    {
        builder.ToTable("LeaveStockOrderRelation");

        builder.Property(x => x.RelationType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationNumber).HasMaxLength(30).IsRequired();

        builder.HasIndex(x => x.LeaveStockOrderItemId).HasDatabaseName("IX_LeaveStockOrderRelation_LeaveStockOrderItemId");
        builder.HasIndex(x => x.RelationId).HasDatabaseName("IX_LeaveStockOrderRelation_RelationId");
        builder.HasIndex(x => x.RelationItemId).HasDatabaseName("IX_LeaveStockOrderRelation_RelationItemId");
        builder.HasIndex(x => x.RelationNumber).HasDatabaseName("IX_LeaveStockOrderRelation_RelationNumber");
    }
}
