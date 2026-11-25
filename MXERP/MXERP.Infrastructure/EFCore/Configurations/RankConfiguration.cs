using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Ranks;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 职级表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class RankConfiguration : IEntityTypeConfiguration<Rank>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Rank> builder)
    {
        builder.ToTable("Rank");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Rank_Name");
    }
}
