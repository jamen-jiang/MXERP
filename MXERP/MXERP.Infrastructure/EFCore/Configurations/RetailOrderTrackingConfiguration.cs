using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.RetailOrders;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 散镜订单跟踪配置
/// </summary>
public sealed class RetailOrderTrackingConfiguration : IEntityTypeConfiguration<RetailOrderTracking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RetailOrderTracking> builder)
    {
        builder.ToTable("RetailOrderTracking");

        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.IsSyncHkoTracking).IsRequired();

        builder.HasIndex(x => x.RetailOrderId).HasDatabaseName("IX_RetailOrderTracking_RetailOrderId");
        builder.HasIndex(x => x.IsSyncHkoTracking).HasDatabaseName("IX_RetailOrderTracking_IsSyncHkoTracking");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_RetailOrderTracking_WorkStepId");
        builder.HasIndex(x => x.EmployeeId).HasDatabaseName("IX_RetailOrderTracking_EmployeeId");
        builder.HasIndex(x => x.OutDepartmentId).HasDatabaseName("IX_RetailOrderTracking_OutDepartmentId");
        builder.HasIndex(x => x.InDepartmentId).HasDatabaseName("IX_RetailOrderTracking_InDepartmentId");
        builder.HasIndex(x => new { x.RetailOrderId, x.WorkStepId }).IsUnique().HasDatabaseName("UX_RetailOrderTracking_RetailOrderId_WorkStepId").HasFilter("[WorkStepId] IS NOT NULL AND [IsDeleted] = 0");
    }
}
