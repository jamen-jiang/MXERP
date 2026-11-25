using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Spus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Spu表映射
/// </summary>
public sealed class SpuConfiguration : IEntityTypeConfiguration<Spu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Spu> builder)
    {
        builder.ToTable("Spu");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Group).HasColumnName("Group").HasMaxLength(50);
        builder.Property(x => x.SupplierId).IsRequired();
        builder.Property(x => x.BrandId);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.PurchasePrice).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.Property(x => x.UnitId).IsRequired();
        builder.Property(x => x.TaxRateId);
        builder.Property(x => x.Moq).HasPrecision(18, 2);
        builder.Property(x => x.Disabled).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.Attributes).HasSortedDictionaryJsonConversion().HasColumnName("Attributes");

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_Spu_ProductType");
        builder.HasIndex(x => x.Group).HasDatabaseName("IX_Spu_Group");
        builder.HasIndex(x => x.SupplierId).HasDatabaseName("IX_Spu_SupplierId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Spu_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Spu_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Spu_Code").HasFilter("[IsDeleted] = 0");
    }
}
