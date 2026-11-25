using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.TaxRates;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 税率表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.ToTable("TaxRate");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Rate).HasPrecision(18, 2);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_TaxRate_Name");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_TaxRate_Sort");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_TaxRate_Code").HasFilter("[IsDeleted] = 0");
    }
}
