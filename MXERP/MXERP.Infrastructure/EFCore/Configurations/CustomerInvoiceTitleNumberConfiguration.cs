using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.CustomerInvoiceTitleNumbers;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// CustomerInvoiceTitleNumber表映射
/// </summary>
public sealed class CustomerInvoiceTitleNumberConfiguration : IEntityTypeConfiguration<CustomerInvoiceTitleNumber>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CustomerInvoiceTitleNumber> builder)
    {
        builder.ToTable("CustomerInvoiceTitleNumber");

        builder.Property(x => x.InvoiceTitle).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Number).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Month).IsRequired();
        builder.Property(x => x.Day).IsRequired();
        builder.Property(x => x.PadNumber).IsRequired();
        builder.Property(x => x.Rule).HasMaxLength(50);

        builder.HasIndex(x => x.Number).HasDatabaseName("IX_CustomerInvoiceTitleNumber_Number");
        builder.HasIndex(x => x.InvoiceTitle).IsUnique().HasDatabaseName("UX_CustomerInvoiceTitleNumber_InvoiceTitle").HasFilter("[IsDeleted] = 0");
    }
}
