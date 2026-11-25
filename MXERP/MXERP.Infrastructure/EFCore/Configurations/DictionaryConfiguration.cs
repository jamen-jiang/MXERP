using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Dictionaries;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 字典实体配置
/// </summary>
public sealed class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dictionary> builder)
    {
        builder.ToTable("Dictionary");
        builder.Property(x => x.Category).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Dictionary_Name");
        builder.HasMany(x => x.DictionaryItems).WithOne().HasForeignKey(i => i.DictionaryId).OnDelete(DeleteBehavior.Cascade);
    }
}
