using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.RetailOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 散镜订单配置
/// </summary>
public sealed class RetailOrderConfiguration : IEntityTypeConfiguration<RetailOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RetailOrder> builder)
    {
        builder.ToTable("RetailOrder");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.OutArea).HasMaxLength(50);
        builder.Property(x => x.SequenceNumber).HasMaxLength(100);
        builder.Property(x => x.HkNumber).HasColumnName("HKNumber").HasMaxLength(100);
        builder.Property(x => x.CustomerOrderNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.DocumentaryBarCode).HasMaxLength(100);
        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.OutGood).IsRequired();
        builder.Property(x => x.OutGoodTime).HasMaxLength(50);
        builder.Property(x => x.Material).HasMaxLength(2);
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.LeaveStockQty).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.InvoicePrintOn);
        builder.Property(x => x.IsPrinted).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.PrintedByName).HasMaxLength(50);
        builder.Property(x => x.CheckedByName).HasMaxLength(50);
        builder.Property(x => x.FinishedByName).HasMaxLength(50);
        builder.Property(x => x.CanceledByName).HasMaxLength(50);
        builder.Property(x => x.IsSyncHkoTracking).IsRequired();

        builder.OwnsOne(x => x.Process, process =>
        {
            process.Property(x => x.Coating).HasColumnName("Coating").HasMaxLength(50);
            process.Property(x => x.TintingColor).HasColumnName("TintingColor").HasMaxLength(50);
            process.Property(x => x.Mirror).HasColumnName("Mirror").HasMaxLength(50);
            process.Property(x => x.TintingCode).HasColumnName("TintingCode").HasMaxLength(50);
            process.Property(x => x.TintingName).HasColumnName("TintingName").HasMaxLength(50);
            process.Property(x => x.Uv).HasColumnName("UV").IsRequired();
            process.Property(x => x.Hard).HasColumnName("Hard").IsRequired();
            process.Property(x => x.Polish).HasColumnName("Polish").IsRequired();
            process.Property(x => x.Auro).HasColumnName("Auro").HasMaxLength(50);
            process.Property(x => x.Edge).HasColumnName("Edge").HasMaxLength(50);
            process.Property(x => x.SuperHole).HasColumnName("Super").HasMaxLength(50);
            process.Property(x => x.Special).HasColumnName("Special").HasMaxLength(50);
            process.Property(x => x.SpecialCode).HasColumnName("SpecialCode").HasMaxLength(50);
            process.Property(x => x.ZuanKong).HasColumnName("ZuanKong").HasMaxLength(50);
            process.Property(x => x.Frame).HasColumnName("Frame").HasMaxLength(50);
        });

        builder.HasMany(x => x.RetailOrderItems).WithOne().HasForeignKey(x => x.RetailOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.RetailOrderTrackings).WithOne().HasForeignKey(x => x.RetailOrderId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_RetailOrder_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_RetailOrder_CompanyId");
        builder.HasIndex(x => x.CustomerOrderNumber).HasDatabaseName("IX_RetailOrder_CustomerOrderNumber");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_RetailOrder_Status");
        builder.HasIndex(x => x.IsSyncHkoTracking).HasDatabaseName("IX_RetailOrder_IsSyncHkoTracking");
        builder.HasIndex(x => new { x.CustomerOrderNumber, x.CustomerPONumber }).IsUnique().HasDatabaseName("UX_RetailOrder_CustomerOrderNumber_CustomerPONumber").HasFilter("[IsDeleted] = 0 AND [Status] <> 'Canceled'");
    }
}
