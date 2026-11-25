using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Emails;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 邮件附件实体配置
/// </summary>
public sealed class EmailAttachmentConfiguration : IEntityTypeConfiguration<EmailAttachment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EmailAttachment> builder)
    {
        builder.ToTable("EmailAttachment");
        builder.Property(x => x.EmailId);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Data);
        builder.HasIndex(x => x.EmailId).HasDatabaseName("IX_EmailAttachment_EmailId");
        builder.HasIndex(x => x.FileName).HasDatabaseName("IX_EmailAttachment_FileName");
    }
}
