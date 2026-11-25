using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Users;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// User_Post表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class UserPostConfiguration : IEntityTypeConfiguration<UserPost>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserPost> builder)
    {
        builder.ToTable("User_Post");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.PostId).IsRequired();

        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_User_Post_UserId");
        builder.HasIndex(x => x.PostId).HasDatabaseName("IX_User_Post_PostId");
    }
}
