using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Attributes;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// AttributeValidateRule表映射
/// </summary>
public sealed class AttributeValidateRuleConfiguration : IEntityTypeConfiguration<AttributeValidateRule>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AttributeValidateRule> builder)
    {
        builder.ToTable("AttributeValidateRule");

        builder.Property(x => x.AttributeId);
        builder.Property(x => x.Expression).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ErrorMessage).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Sort);

        builder.HasIndex(x => x.AttributeId).HasDatabaseName("IX_AttributeValidateRule_AttributeId");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_AttributeValidateRule_Sort");
    }
}
