using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveWarehouseOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出仓单实体配置
/// </summary>
public class LeaveWarehouseOrderConfiguration : IEntityTypeConfiguration<LeaveWarehouseOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveWarehouseOrder> builder)
    {
        builder.ToTable("LeaveWarehouseOrder");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Type).HasColumnName("Type").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Sender).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SenderTelephone).HasMaxLength(50);
        builder.Property(x => x.SendArea).HasMaxLength(50);
        builder.Property(x => x.SendAddress).HasMaxLength(500);
        builder.Property(x => x.Deliveryer).HasMaxLength(50).IsRequired();
        builder.Property(x => x.DeliveryerTelephone).HasMaxLength(50);
        builder.Property(x => x.DeliveryArea).HasMaxLength(50);
        builder.Property(x => x.DeliveryAddress).HasMaxLength(500);
        builder.Property(x => x.Money).HasPrecision(18, 2);
        builder.Property(x => x.PrintedByName).HasMaxLength(50);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_LeaveWarehouseOrder_ProductType");
        builder.HasIndex(x => x.Type).HasDatabaseName("IX_LeaveWarehouseOrder_Type");
        builder.HasIndex(x => x.IsPrinted).HasDatabaseName("IX_LeaveWarehouseOrder_IsPrinted");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_LeaveWarehouseOrder_CompanyId");

        builder.HasMany(x => x.LeaveWarehouseOrderItems)
            .WithOne()
            .HasForeignKey(x => x.LeaveWarehouseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
