using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExternalLeaveStockOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 外部出库单跟踪配置
/// </summary>
public sealed class ExternalLeaveStockOrderTrackingConfiguration : IEntityTypeConfiguration<ExternalLeaveStockOrderTracking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExternalLeaveStockOrderTracking> builder)
    {
        builder.ToTable("ExternalLeaveStockOrderTracking");

        builder.Property(x => x.Reason).HasMaxLength(500);

        builder.HasIndex(x => x.ExternalLeaveStockOrderId).HasDatabaseName("IX_ExternalLeaveStockOrderTracking_ExternalLeaveStockOrderId");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_ExternalLeaveStockOrderTracking_WorkStepId");
        builder.HasIndex(x => x.EmployeeId).HasDatabaseName("IX_ExternalLeaveStockOrderTracking_EmployeeId");
        builder.HasIndex(x => x.OutDepartmentId).HasDatabaseName("IX_ExternalLeaveStockOrderTracking_OutDepartmentId");
        builder.HasIndex(x => x.InDepartmentId).HasDatabaseName("IX_ExternalLeaveStockOrderTracking_InDepartmentId");
    }
}
