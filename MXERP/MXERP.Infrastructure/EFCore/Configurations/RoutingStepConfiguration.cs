using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Routings;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// RoutingStep表映射
/// </summary>
public sealed class RoutingStepConfiguration : IEntityTypeConfiguration<RoutingStep>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RoutingStep> builder)
    {
        builder.ToTable("RoutingStep");

        builder.Property(x => x.RoutingId);
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.WorkStepId).IsRequired();
        builder.Property(x => x.IsMust).IsRequired();
        builder.Property(x => x.TimeInMins);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.RoutingId).HasDatabaseName("IX_RoutingStep_RoutingId");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_RoutingStep_WorkStepId");
    }
}
