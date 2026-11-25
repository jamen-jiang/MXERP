using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Attachments;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Attachment表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachment");

        builder.Property(x => x.OwnerType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.FileExtend).IsRequired().HasMaxLength(10);
        builder.Property(x => x.BasePath).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.SavePath).IsRequired().HasMaxLength(1000);

        builder.HasIndex(x => x.OwnerId).HasDatabaseName("IX_Attachment_OwnerId");
        builder.HasIndex(x => x.OwnerType).HasDatabaseName("IX_Attachment_OwnerType");
    }
}
