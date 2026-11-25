using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Posts;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Post_Role表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class PostRoleConfiguration : IEntityTypeConfiguration<PostRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PostRole> builder)
    {
        builder.ToTable("Post_Role");

        builder.Property(x => x.PostId).IsRequired();
        builder.Property(x => x.RoleId).IsRequired();

        builder.HasIndex(x => x.PostId).HasDatabaseName("IX_Post_Role_PostId");
        builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_Post_Role_RoleId");
    }
}
