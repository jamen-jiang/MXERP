using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Attributes;
using AttributeEntity = MXERP.Domain.Attributes.Attribute;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Attribute表映射
/// </summary>
public sealed class AttributeConfiguration : IEntityTypeConfiguration<AttributeEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AttributeEntity> builder)
    {
        builder.ToTable("Attribute");

        builder.Property(x => x.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Scope).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IsKey).IsRequired();
        builder.Property(x => x.InputType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DataType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IsRequired).IsRequired();
        builder.Property(x => x.IsFilter).IsRequired();
        builder.Property(x => x.Sort);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.ProductType).HasDatabaseName("IX_Attribute_ProductType");
        builder.HasIndex(x => x.Scope).HasDatabaseName("IX_Attribute_Scope");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Attribute_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Attribute_Name");
        builder.HasIndex(x => x.Sort).HasDatabaseName("IX_Attribute_Sort");
        builder.HasIndex(x => new { x.ProductType, x.Scope, x.Code }).IsUnique().HasDatabaseName("UX_Attribute_ProductType_Scope_Code").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.AttributeItems)
            .WithOne()
            .HasForeignKey(x => x.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AttributeValidateRules)
            .WithOne()
            .HasForeignKey(x => x.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
