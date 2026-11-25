using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.OrderEvents;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 订单事件配置
/// </summary>
public sealed class OrderEventConfiguration : IEntityTypeConfiguration<OrderEvent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrderEvent> builder)
    {
        builder.ToTable("OrderEvent");

        builder.Property(x => x.OrderType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.OrderStatus).HasMaxLength(50).IsRequired();
        builder.Property(x => x.To).HasMaxLength(2000);
        builder.Property(x => x.Cc).HasMaxLength(2000);
        builder.Property(x => x.Subject).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Template).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.OrderType).HasDatabaseName("IX_OrderEvent_OrderType");
        builder.HasIndex(x => x.OrderStatus).HasDatabaseName("IX_OrderEvent_OrderStatus");

        builder.HasMany(x => x.OrderEventDepartments)
            .WithOne()
            .HasForeignKey(x => x.OrderEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderEventItems)
            .WithOne()
            .HasForeignKey(x => x.OrderEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
