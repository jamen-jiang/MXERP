using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.EnterStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 入库单配置
/// </summary>
public class EnterStockOrderConfiguration : IEntityTypeConfiguration<EnterStockOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EnterStockOrder> builder)
    {
        builder.ToTable("EnterStockOrder");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationType).HasMaxLength(50);
        builder.Property(x => x.BusinessType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.EnterStockOrderItems)
            .WithOne()
            .HasForeignKey(x => x.EnterStockOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_EnterStockOrder_ProductType");
        builder.HasIndex(x => x.OriginalId).HasDatabaseName("IX_EnterStockOrder_OriginalId");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_EnterStockOrder_RelationType");
        builder.HasIndex(x => x.BusinessType).HasDatabaseName("IX_EnterStockOrder_BusinessType");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_EnterStockOrder_Status");
    }
}
