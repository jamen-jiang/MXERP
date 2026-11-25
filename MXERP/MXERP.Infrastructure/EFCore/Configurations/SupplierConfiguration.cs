using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Suppliers;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 供应商表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Supplier");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.OldCode).HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Number).HasMaxLength(50);
        builder.Property(x => x.ShortName).HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Telephone).HasMaxLength(100);
        builder.Property(x => x.Fax).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.ContactPerson).HasMaxLength(30);
        builder.Property(x => x.PayMode).HasMaxLength(200);
        builder.Property(x => x.DeliveryMode).HasMaxLength(200);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Supplier_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Supplier_Code").HasFilter("[IsDeleted] = 0");
    }
}
