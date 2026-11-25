using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Companies;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// CompanyContact表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class CompanyContactConfiguration : IEntityTypeConfiguration<CompanyContact>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CompanyContact> builder)
    {
        builder.ToTable("CompanyContact");

        builder.Property(x => x.CompanyId);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Telephone).HasMaxLength(50);
        builder.Property(x => x.Area).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(800);

        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_CompanyContact_CompanyId");
    }
}
