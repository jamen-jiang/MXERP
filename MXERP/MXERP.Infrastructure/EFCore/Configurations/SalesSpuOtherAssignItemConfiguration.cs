using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSpuOtherAssignItems;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSpuOtherAssignItem表映射
/// </summary>
public sealed class SalesSpuOtherAssignItemConfiguration : IEntityTypeConfiguration<SalesSpuOtherAssignItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSpuOtherAssignItem> builder)
    {
        builder.ToTable("SalesSpuOtherAssignItem");

        builder.Property(x => x.SalesSpuId).IsRequired();
        builder.Property(x => x.Coating).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();

        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_SalesSpuOtherAssignItem_SalesSpuId");
        builder.HasIndex(x => x.Coating).HasDatabaseName("IX_SalesSpuOtherAssignItem_Coating");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_SalesSpuOtherAssignItem_SpuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_SalesSpuOtherAssignItem_WarehouseId");
    }
}
