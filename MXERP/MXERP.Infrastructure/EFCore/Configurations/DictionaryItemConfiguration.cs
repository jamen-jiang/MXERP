using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Dictionaries;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 字典项实体配置
/// </summary>
public sealed class DictionaryItemConfiguration : IEntityTypeConfiguration<DictionaryItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DictionaryItem> builder)
    {
        builder.ToTable("DictionaryItem");
        builder.Property(x => x.DictionaryId);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Label).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Sort);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.HasIndex(x => x.DictionaryId).HasDatabaseName("IX_DictionaryItem_DictionaryId");
        builder.HasIndex(x => x.Value).HasDatabaseName("IX_DictionaryItem_Value");
        builder.HasIndex(x => x.Label).HasDatabaseName("IX_DictionaryItem_Label");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_DictionaryItem_Sort");
    }
}
