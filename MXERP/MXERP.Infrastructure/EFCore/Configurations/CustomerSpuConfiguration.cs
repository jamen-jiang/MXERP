using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.CustomersSpus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// CustomerSpu表映射
/// </summary>
public sealed class CustomerSpuConfiguration : IEntityTypeConfiguration<CustomerSpu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CustomerSpu> builder)
    {
        builder.ToTable("CustomerSpu");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SalesSpuId).IsRequired();
        builder.Property(x => x.BrandId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.BagNumber).HasMaxLength(50);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_CustomerSpu_ProductType");
        builder.HasIndex(x => x.SalesSpuId).HasDatabaseName("IX_CustomerSpu_SalesSpuId");
        builder.HasIndex(x => x.BrandId).HasDatabaseName("IX_CustomerSpu_BrandId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_CustomerSpu_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_CustomerSpu_Name");
    }
}
