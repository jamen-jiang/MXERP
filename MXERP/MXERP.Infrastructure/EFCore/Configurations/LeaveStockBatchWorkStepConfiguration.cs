using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.LeaveStockBatches;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 出库批次工序配置
/// </summary>
public sealed class LeaveStockBatchWorkStepConfiguration : IEntityTypeConfiguration<LeaveStockBatchWorkStep>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveStockBatchWorkStep> builder)
    {
        builder.ToTable("LeaveStockBatchWorkStep");

        builder.Property(x => x.WorkStepId).IsRequired();

        builder.HasIndex(x => x.LeaveStockBatchId).HasDatabaseName("IX_LeaveStockBatchWorkStep_LeaveStockBatchId");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_LeaveStockBatchWorkStep_WorkStepId");
    }
}
