using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSpus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSpuAssignItem表映射
/// </summary>
public sealed class SalesSpuAssignItemConfiguration : IEntityTypeConfiguration<SalesSpuAssignItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSpuAssignItem> builder)
    {
        builder.ToTable("SalesSpuAssignItem");

        builder.Property(x => x.SalesSpuId);
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.IsPriorityCustomized).IsRequired();

        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_SalesSpuAssignItem_SalesSpuId");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_SalesSpuAssignItem_SpuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_SalesSpuAssignItem_WarehouseId");
    }
}
