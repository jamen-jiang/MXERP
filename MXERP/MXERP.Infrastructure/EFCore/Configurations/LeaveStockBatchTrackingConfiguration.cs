using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库批次跟踪配置
/// </summary>
public sealed class LeaveStockBatchTrackingConfiguration : IEntityTypeConfiguration<LeaveStockBatchTracking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockBatchTracking> builder)
    {
        builder.ToTable("LeaveStockBatchTracking");

        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.OutDepartmentId).IsRequired();
        builder.Property(x => x.InDepartmentId).IsRequired();

        builder.HasIndex(x => x.LeaveStockBatchId).HasDatabaseName("IX_LeaveStockBatchTracking_LeaveStockBatchId");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_LeaveStockBatchTracking_WorkStepId");
        builder.HasIndex(x => x.EmployeeId).HasDatabaseName("IX_LeaveStockBatchTracking_EmployeeId");
        builder.HasIndex(x => x.OutDepartmentId).HasDatabaseName("IX_LeaveStockBatchTracking_OutDepartmentId");
        builder.HasIndex(x => x.InDepartmentId).HasDatabaseName("IX_LeaveStockBatchTracking_InDepartmentId");
    }
}
