using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Stores;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// StoreContact表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class StoreContactConfiguration : IEntityTypeConfiguration<StoreContact>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StoreContact> builder)
    {
        builder.ToTable("StoreContact");

        builder.Property(x => x.StoreId);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.Telephone).HasMaxLength(50);
        builder.Property(x => x.Area).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(800);

        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_StoreContact_StoreId");
    }
}
