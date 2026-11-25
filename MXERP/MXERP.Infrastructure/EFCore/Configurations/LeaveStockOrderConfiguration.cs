using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库单配置
/// </summary>
public class LeaveStockOrderConfiguration : IEntityTypeConfiguration<LeaveStockOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockOrder> builder)
    {
        builder.ToTable("LeaveStockOrder");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RelationType).HasMaxLength(50);
        builder.Property(x => x.BusinessType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);

        builder.HasMany(x => x.LeaveStockOrderItems)
            .WithOne()
            .HasForeignKey(x => x.LeaveStockOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_LeaveStockOrder_ProductType");
        builder.HasIndex(x => x.OriginalId).HasDatabaseName("IX_LeaveStockOrder_OriginalId");
        builder.HasIndex(x => x.RelationType).HasDatabaseName("IX_LeaveStockOrder_RelationType");
        builder.HasIndex(x => x.BusinessType).HasDatabaseName("IX_LeaveStockOrder_BusinessType");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_LeaveStockOrder_Status");
    }
}
