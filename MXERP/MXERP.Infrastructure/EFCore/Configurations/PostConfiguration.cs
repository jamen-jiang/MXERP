using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Posts;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Post表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.RankId).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Post_Name");
        builder.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_Post_DepartmentId");
        builder.HasIndex(x => x.RankId).HasDatabaseName("IX_Post_RankId");

        builder.HasMany(x => x.PostRoles)
            .WithOne()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
