using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Brands;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>品牌表映射(对应 Java 的 @Table/@Column/@Index)</summary>
public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brand");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Brand_Name");

        // Code 唯一, 但只约束未删除的行(过滤唯一索引 WHERE IsDeleted = 0)。
        // Java 侧因 JPA 不支持过滤索引而改用 Flyway 建; EF 的 Fluent 直接能表达, 不必额外脚本。
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_Brand_Code")
            .HasFilter("[IsDeleted] = 0");
    }
}
