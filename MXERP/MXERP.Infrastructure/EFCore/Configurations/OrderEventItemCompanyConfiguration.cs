using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.OrderEvents;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 订单事件项公司配置
/// </summary>
public sealed class OrderEventItemCompanyConfiguration : IEntityTypeConfiguration<OrderEventItemCompany>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrderEventItemCompany> builder)
    {
        builder.ToTable("OrderEventItem_Company");

        builder.Property(x => x.CompanyId).IsRequired();

        builder.HasIndex(x => x.OrderEventItemId).HasDatabaseName("IX_OrderEventItem_Company_OrderEventItemId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_OrderEventItem_Company_CompanyId");
    }
}
