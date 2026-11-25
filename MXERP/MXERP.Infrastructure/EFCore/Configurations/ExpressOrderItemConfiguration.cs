using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExpressOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 快递单项配置
/// </summary>
public sealed class ExpressOrderItemConfiguration : IEntityTypeConfiguration<ExpressOrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExpressOrderItem> builder)
    {
        builder.ToTable("ExpressOrderItem");

        builder.Property(x => x.Grid).HasMaxLength(50).IsRequired();
        builder.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerPONumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LensName).HasMaxLength(400).IsRequired();
        builder.Property(x => x.Qty).IsRequired();

        builder.HasIndex(x => x.ExpressOrderId).HasDatabaseName("IX_ExpressOrderItem_ExpressOrderId");
        builder.HasIndex(x => x.Grid).HasDatabaseName("IX_ExpressOrderItem_Grid");
        builder.HasIndex(x => x.OrderNumber).HasDatabaseName("IX_ExpressOrderItem_OrderNumber");
        builder.HasIndex(x => x.CustomerPONumber).HasDatabaseName("IX_ExpressOrderItem_CustomerPONumber");
        builder.HasIndex(x => x.LensName).HasDatabaseName("IX_ExpressOrderItem_LensName");
    }
}
