using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSpus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSpuRouting表映射
/// </summary>
public sealed class SalesSpuRoutingConfiguration : IEntityTypeConfiguration<SalesSpuRouting>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSpuRouting> builder)
    {
        builder.ToTable("SalesSpuRouting");

        builder.Property(x => x.SalesSpuId);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.RoutingId).IsRequired();

        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_SalesSpuRouting_SalesSpuId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_SalesSpuRouting_SpuId");
        builder.HasIndex(x => x.RoutingId).HasDatabaseName("IX_SalesSpuRouting_RoutingId");
        builder.HasIndex(x => new { x.SalesSpuId, x.SpuId }).IsUnique().HasDatabaseName("UX_SalesSpuRouting_SalesSpuId_SpuId").HasFilter("[IsDeleted] = 0");
    }
}
