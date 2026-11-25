using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExpressOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 快递单配置
/// </summary>
public sealed class ExpressOrderConfiguration : IEntityTypeConfiguration<ExpressOrder>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExpressOrder> builder)
    {
        builder.ToTable("ExpressOrder");

        builder.Property(x => x.Number).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired();

        builder.HasMany(x => x.ExpressOrderItems).WithOne().HasForeignKey(x => x.ExpressOrderId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Number).HasDatabaseName("IX_ExpressOrder_Number");
        builder.HasIndex(x => x.CustomerCode).HasDatabaseName("IX_ExpressOrder_CustomerCode");
    }
}
