using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SpuBrands;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SpuBrand表映射
/// </summary>
public sealed class SpuBrandConfiguration : IEntityTypeConfiguration<SpuBrand>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SpuBrand> builder)
    {
        builder.ToTable("SpuBrand");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_SpuBrand_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_SpuBrand_Code").HasFilter("[IsDeleted] = 0");
    }
}
