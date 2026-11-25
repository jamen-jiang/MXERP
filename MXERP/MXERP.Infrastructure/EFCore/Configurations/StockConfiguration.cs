using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Stocks;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 库存配置
/// </summary>
public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stock");

        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.SkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.Qty).IsRequired();
        builder.Property(x => x.FreezeQty).IsRequired();
        builder.Property(x => x.UpperLimitQty).IsRequired();
        builder.Property(x => x.LowerLimitQty).IsRequired();
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_Stock_ProductType");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_Stock_SpuId");
        builder.HasIndex(x => x.SkuId).HasDatabaseName("IX_Stock_SkuId");
        builder.HasIndex(x => x.WarehouseId).HasDatabaseName("IX_Stock_WarehouseId");
    }
}
