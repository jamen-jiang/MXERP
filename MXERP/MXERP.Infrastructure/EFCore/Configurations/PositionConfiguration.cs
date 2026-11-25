using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Positions;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 职位表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Position");
        builder.Property(x => x.Category).HasMaxLength(50);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShortName).HasMaxLength(200);
        builder.Property(x => x.EndProductCode).HasMaxLength(50);
        builder.Property(x => x.SemiProductCode).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Position_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Position_Code").HasFilter("[IsDeleted] = 0");
    }
}
