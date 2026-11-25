using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.TripleInvoices;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 三联发票配置
/// </summary>
public sealed class TripleInvoiceConfiguration : IEntityTypeConfiguration<TripleInvoice>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TripleInvoice> builder)
    {
        builder.ToTable("TripleInvoice");

        builder.Property(x => x.Number).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CustomerInvoiceTitleNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.RetailOrderNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.RetailOrderCustomerOrderNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RetailOrderCustomerPONumber).HasMaxLength(100);
        builder.Property(x => x.StoreCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StoreNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StoreInvoiceTitle).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StoreAddress).HasMaxLength(800);
        builder.Property(x => x.StoreTelephone).HasMaxLength(50);
        builder.Property(x => x.SalesSpuCodeR).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerSpuCodeR).HasMaxLength(100);
        builder.Property(x => x.DegreesTypeR).HasMaxLength(5).IsRequired();
        builder.Property(x => x.BaseR).HasMaxLength(5);
        builder.Property(x => x.DecenterR).HasMaxLength(200);
        builder.Property(x => x.PrismR).HasMaxLength(200);
        builder.Property(x => x.SalesSpuCodeL).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerSpuCodeL).HasMaxLength(100);
        builder.Property(x => x.DegreesTypeL).HasMaxLength(5).IsRequired();
        builder.Property(x => x.BaseL).HasMaxLength(5);
        builder.Property(x => x.DecenterL).HasMaxLength(200);
        builder.Property(x => x.PrismL).HasMaxLength(200);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_TripleInvoice_StoreId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_TripleInvoice_CompanyId");
        builder.HasIndex(x => x.RetailOrderId).IsUnique().HasDatabaseName("UX_TripleInvoice_RetailOrderId").HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => x.RetailOrderNumber).IsUnique().HasDatabaseName("UX_TripleInvoice_RetailOrderNumber").HasFilter("[IsDeleted] = 0");
    }
}
