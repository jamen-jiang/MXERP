using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Stores;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Store表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Store");

        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShortName).HasMaxLength(50);
        builder.Property(x => x.Number).HasMaxLength(50);
        builder.Property(x => x.InvoiceTitle).HasMaxLength(50);
        builder.Property(x => x.Telephone).HasMaxLength(50);
        builder.Property(x => x.Fax).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.CityCode).HasMaxLength(20);
        builder.Property(x => x.CityName).HasMaxLength(50);
        builder.Property(x => x.AreaCode).HasMaxLength(20);
        builder.Property(x => x.PostCode).HasMaxLength(20);
        builder.Property(x => x.LogisticsCode).HasMaxLength(20);
        builder.Property(x => x.LogisticsName).HasMaxLength(50);
        builder.Property(x => x.Signer).HasMaxLength(50);
        builder.Property(x => x.ConvertCode).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(800);
        builder.Property(x => x.BillingAddress).HasMaxLength(800);

        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_Store_CompanyId");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Store_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Store_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Store_Code").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.StoreContacts)
            .WithOne()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
