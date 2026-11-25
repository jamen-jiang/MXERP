using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SerialNumbers;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SerialNumber表映射
/// </summary>
public sealed class SerialNumberConfiguration : IEntityTypeConfiguration<SerialNumber>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SerialNumber> builder)
    {
        builder.ToTable("SerialNumber");

        builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Number).IsRequired();
        builder.Property(x => x.Prefix).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Month).IsRequired();
        builder.Property(x => x.Day).IsRequired();
        builder.Property(x => x.DateFormat).HasMaxLength(50);
        builder.Property(x => x.Pad).IsRequired();
        builder.Property(x => x.PadSymbol).IsRequired().HasMaxLength(1);
        builder.Property(x => x.IsSplit).IsRequired();
        builder.Property(x => x.SplitSymbol).IsRequired().HasMaxLength(1);
        builder.Property(x => x.Rule).HasMaxLength(50);

        builder.HasIndex(x => x.Number).HasDatabaseName("IX_SerialNumber_Number");
        builder.HasIndex(x => x.Type).IsUnique().HasDatabaseName("UX_SerialNumber_Type").HasFilter("[IsDeleted] = 0");
    }
}
