using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSpus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSpuWorkStep表映射
/// </summary>
public sealed class SalesSpuWorkStepConfiguration : IEntityTypeConfiguration<SalesSpuWorkStep>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSpuWorkStep> builder)
    {
        builder.ToTable("SalesSpuWorkStep");

        builder.Property(x => x.SalesSpuId);
        builder.Property(x => x.WorkStepId).IsRequired();

        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_SalesSpuWorkStep_SalesSpuId");
        builder.HasIndex(x => x.WorkStepId).HasDatabaseName("IX_SalesSpuWorkStep_WorkStepId");
    }
}
