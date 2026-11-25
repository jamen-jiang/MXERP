using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Diameters;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 直径表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class DiameterConfiguration : IEntityTypeConfiguration<Diameter>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Diameter> builder)
    {
        builder.ToTable("Diameter");
        builder.Property(x => x.Color).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Color).HasDatabaseName("IX_Diameter_Color");
        builder.HasIndex(x => x.IsDefault).HasDatabaseName("IX_Diameter_IsDefault");
        builder.HasIndex(x => x.Size).IsUnique().HasDatabaseName("UX_Diameter_Size").HasFilter("[IsDeleted] = 0");
    }
}
