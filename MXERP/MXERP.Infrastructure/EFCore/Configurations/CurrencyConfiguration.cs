using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Currencies;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 货币表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currency");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Symbol).IsRequired().HasMaxLength(10);
        builder.Property(x => x.AcceptRate).HasPrecision(18, 6);
        builder.Property(x => x.AdjustRate).HasPrecision(18, 6);
        builder.Property(x => x.IsStandard).IsRequired();
        builder.Property(x => x.IsEnable).IsRequired();

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Currency_Name");
        builder.HasIndex(x => x.IsEnable).HasDatabaseName("IX_Currency_IsEnable");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Currency_Code").HasFilter("[IsDeleted] = 0");
    }
}
