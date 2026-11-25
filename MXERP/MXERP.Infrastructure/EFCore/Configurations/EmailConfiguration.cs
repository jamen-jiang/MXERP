using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Emails;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 邮件实体配置
/// </summary>
public sealed class EmailConfiguration : IEntityTypeConfiguration<Email>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Email> builder)
    {
        builder.ToTable("Email");
        builder.Property(x => x.From).IsRequired().HasMaxLength(200).HasColumnName("From");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.To).IsRequired().HasMaxLength(2000).HasColumnName("To");
        builder.Property(x => x.Cc).HasMaxLength(2000);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.IsSuccess).IsRequired();
        builder.Property(x => x.ErrorMessage);
        builder.Property(x => x.ExceptionText).HasColumnName("Exception");
        builder.Property(x => x.SendedOn);
        builder.HasIndex(x => x.From).HasDatabaseName("IX_Email_From");
        builder.HasIndex(x => x.Cc).HasDatabaseName("IX_Email_Cc");
        builder.HasIndex(x => x.Subject).HasDatabaseName("IX_Email_Subject");
        builder.HasIndex(x => x.To).HasDatabaseName("IX_Email_To");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Email_Name");
        builder.HasIndex(x => x.IsSuccess).HasDatabaseName("IX_Email_IsSuccess");
        builder.HasIndex(x => x.SendedOn).IsDescending().HasDatabaseName("IX_Email_SendedOn");
        builder.HasMany(x => x.EmailAttachments).WithOne().HasForeignKey(a => a.EmailId).OnDelete(DeleteBehavior.Cascade);
    }
}
