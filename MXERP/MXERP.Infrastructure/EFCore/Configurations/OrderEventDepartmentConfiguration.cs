using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.OrderEvents;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 订单事件绑定的组织机构配置
/// </summary>
public sealed class OrderEventDepartmentConfiguration : IEntityTypeConfiguration<OrderEventDepartment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrderEventDepartment> builder)
    {
        builder.ToTable("OrderEvent_Department");

        builder.Property(x => x.DepartmentId).IsRequired();

        builder.HasIndex(x => x.OrderEventId).HasDatabaseName("IX_OrderEvent_Department_OrderEventId");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_OrderEvent_Department_DepartmentId");
    }
}
