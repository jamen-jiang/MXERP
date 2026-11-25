using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.ExternalRetailOrderTrackings;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 外部散镜订单跟踪配置
/// </summary>
public sealed class ExternalRetailOrderTrackingConfiguration : IEntityTypeConfiguration<ExternalRetailOrderTracking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExternalRetailOrderTracking> builder)
    {
        builder.ToTable("ExternalRetailOrderTracking");

        builder.Property(x => x.ExternalRetailOrderNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.OutDepartmentId).IsRequired();
        builder.Property(x => x.InDepartmentId).IsRequired();

        builder.HasIndex(x => x.ExternalRetailOrderNumber).HasDatabaseName("IX_ExternalRetailOrderTracking_ExternalRetailOrderNumber");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_ExternalRetailOrderTracking_WorkStepId");
        builder.HasIndex(x => x.EmployeeId).HasDatabaseName("IX_ExternalRetailOrderTracking_EmployeeId");
        builder.HasIndex(x => x.OutDepartmentId).HasDatabaseName("IX_ExternalRetailOrderTracking_OutDepartmentId");
        builder.HasIndex(x => x.InDepartmentId).HasDatabaseName("IX_ExternalRetailOrderTracking_InDepartmentId");
    }
}
