using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Attributes;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// AttributeItem表映射
/// </summary>
public sealed class AttributeItemConfiguration : IEntityTypeConfiguration<AttributeItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AttributeItem> builder)
    {
        builder.ToTable("AttributeItem");

        builder.Property(x => x.AttributeId);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.Sort);

        builder.HasIndex(x => x.AttributeId).HasDatabaseName("IX_AttributeItem_AttributeId");
        builder.HasIndex(x => x.Value).HasDatabaseName("IX_AttributeItem_Value");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_AttributeItem_Sort");
        builder.HasIndex(x => new { x.AttributeId, x.Value }).IsUnique().HasDatabaseName("UX_AttributeItem_AttributeId_Value").HasFilter("[IsDeleted] = 0");
    }
}
