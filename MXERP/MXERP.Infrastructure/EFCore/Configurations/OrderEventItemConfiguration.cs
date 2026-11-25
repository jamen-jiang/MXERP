using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.OrderEvents;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 订单事件项配置
/// </summary>
public sealed class OrderEventItemConfiguration : IEntityTypeConfiguration<OrderEventItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrderEventItem> builder)
    {
        builder.ToTable("OrderEventItem");

        builder.Property(x => x.To).HasMaxLength(2000);
        builder.Property(x => x.Cc).HasMaxLength(2000);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.OrderEventId).HasDatabaseName("IX_OrderEventItem_OrderEventId");

        builder.HasMany(x => x.OrderEventItemCompanys)
            .WithOne()
            .HasForeignKey(x => x.OrderEventItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
