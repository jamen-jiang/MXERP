using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.EndProductCodeConfigs;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// EndProductCodeConfig表映射
/// </summary>
public sealed class EndProductCodeConfigConfiguration : IEntityTypeConfiguration<EndProductCodeConfig>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EndProductCodeConfig> builder)
    {
        builder.ToTable("EndProductCodeConfig");

        builder.Property(x => x.Category).IsRequired().HasMaxLength(20);
        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.DegreesType).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Sort);

        builder.HasIndex(x => x.Category).HasDatabaseName("IX_EndProductCodeConfig_Category");
        builder.HasIndex(x => new { x.SpuId, x.DegreesType }).IsUnique().HasDatabaseName("UX_EndProductCodeConfig_SpuId_DegreesType").HasFilter("[IsDeleted] = 0");
    }
}
