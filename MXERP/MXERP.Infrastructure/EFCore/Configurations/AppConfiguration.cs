using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Apps;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// App表映射(对应 Java 的 @Table/@Column/@Index, 含内嵌 MetaVO)
/// </summary>
public sealed class AppConfiguration : IEntityTypeConfiguration<App>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<App> builder)
    {
        builder.ToTable("App");

        builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ParentId);
        builder.Property(x => x.Level);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Path).HasMaxLength(500);
        builder.Property(x => x.Component).HasMaxLength(500);
        builder.Property(x => x.Redirect).HasMaxLength(500);
        builder.Property(x => x.License).HasMaxLength(200);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_App_Name");
        builder.HasIndex(x => x.Type).HasDatabaseName("IX_App_Type");

        builder.OwnsOne(x => x.Meta, meta =>
        {
            meta.Property(m => m.Title).HasColumnName("Title").IsRequired().HasMaxLength(200);
            meta.Property(m => m.I18nKey).HasColumnName("I18nKey").HasMaxLength(200);
            meta.Property(m => m.Icon).HasColumnName("Icon").HasMaxLength(500);
            meta.Property(m => m.IsKeepAlive).HasColumnName("IsKeepAlive").IsRequired();
            meta.Property(m => m.FixedIndexInTab).HasColumnName("FixedIndexInTab");
            meta.Property(m => m.IsMultiTab).HasColumnName("IsMultiTab").IsRequired();
            meta.Property(m => m.IsHide).HasColumnName("IsHide").IsRequired();
            meta.Property(m => m.Sort).HasColumnName("Sort");
        });
    }
}
