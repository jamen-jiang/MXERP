using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SalesSpus;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SalesSpu表映射
/// </summary>
public sealed class SalesSpuConfiguration : IEntityTypeConfiguration<SalesSpu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesSpu> builder)
    {
        builder.ToTable("SalesSpu");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.BulkOrderPacketInfo).HasMaxLength(500);
        builder.Property(x => x.RetailOrderPacketInfo).HasMaxLength(500);
        builder.Property(x => x.Attributes).HasSortedDictionaryJsonConversion().HasColumnName("Attributes");

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_SalesSpu_ProductType");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_SalesSpu_CompanyId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_SalesSpu_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_SalesSpu_Name");
        builder.HasIndex(x => new { x.ProductType, x.CompanyId, x.Code }).IsUnique().HasDatabaseName("UX_SalesSpu_ProductType_CompanyId_Code").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.SalesSpuAssignItems).WithOne().HasForeignKey(x => x.SalesSpuId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.SalesSpuWorkSteps).WithOne().HasForeignKey(x => x.SalesSpuId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.SalesSpuRoutings).WithOne().HasForeignKey(x => x.SalesSpuId).OnDelete(DeleteBehavior.Cascade);
    }
}
