using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Skus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Sku表映射
/// </summary>
public sealed class SkuConfiguration : IEntityTypeConfiguration<Sku>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Sku> builder)
    {
        builder.ToTable("Sku");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.AttributesSignature).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Opc).HasMaxLength(100);
        builder.Property(x => x.PurchasePrice).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyId);
        builder.Property(x => x.UnitId);
        builder.Property(x => x.TaxRateId);
        builder.Property(x => x.Disabled).IsRequired();
        builder.Property(x => x.Moq).HasPrecision(18, 2);
        builder.Property(x => x.KeyAttributes).HasSortedDictionaryJsonConversion().HasColumnName("KeyAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_Sku_ProductType");
        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_Sku_SpuId");
        builder.HasIndex(x => x.Opc).HasDatabaseName("IX_Sku_Opc");
        builder.HasIndex(x => x.AttributesSignature).IsUnique().HasDatabaseName("UX_Sku_AttributesSignature").HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.SpuId, x.Opc }).IsUnique().HasDatabaseName("UX_Sku_SpuId_Opc").HasFilter("[IsDeleted] = 0 AND [Opc] IS NOT NULL AND [Opc] <> ''");
    }
}
