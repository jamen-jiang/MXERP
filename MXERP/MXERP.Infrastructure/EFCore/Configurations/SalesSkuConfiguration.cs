using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSkus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSku表映射
/// </summary>
public sealed class SalesSkuConfiguration : IEntityTypeConfiguration<SalesSku>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSku> builder)
    {
        builder.ToTable("SalesSku");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.SalesSpuId).IsRequired();
        builder.Property(x => x.AttributesSignature).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Opc).HasMaxLength(100);
        builder.Property(x => x.KeyAttributes).HasSortedDictionaryJsonConversion().HasColumnName("KeyAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_SalesSku_ProductType");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_SalesSku_CompanyId");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_SalesSku_SalesSpuId");
        builder.HasIndex(x => x.Opc).HasDatabaseName("IX_SalesSku_Opc");
        builder.HasIndex(x => x.AttributesSignature).IsUnique().HasDatabaseName("UX_SalesSku_AttributesSignature").HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CompanyId, x.Opc }).IsUnique().HasDatabaseName("UX_SalesSku_CompanyId_Opc").HasFilter("[IsDeleted] = 0 AND [Opc] IS NOT NULL AND [Opc] <> ''");
    }
}
