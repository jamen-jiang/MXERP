using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SignInLogs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SignInLog表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class SignInLogConfiguration : IEntityTypeConfiguration<SignInLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SignInLog> builder)
    {
        builder.ToTable("SignInLog");

        builder.Property(x => x.Type).IsRequired().HasMaxLength(50).HasColumnName("Type");
        builder.Property(x => x.UserCode).HasMaxLength(50);
        builder.Property(x => x.Token).HasMaxLength(500);
        builder.Property(x => x.RefreshToken).HasMaxLength(500);
        builder.Property(x => x.OldRefreshToken).HasMaxLength(500);
        builder.Property(x => x.Ip).HasMaxLength(200).HasColumnName("IP");
        builder.Property(x => x.City).HasMaxLength(50);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.Browser).HasMaxLength(200);
        builder.Property(x => x.Os).HasMaxLength(200);

        builder.HasIndex(x => x.Type).HasDatabaseName("IX_SignInLog_Type");
        builder.HasIndex(x => x.UserCode).HasDatabaseName("IX_SignInLog_UserCode");
    }
}
