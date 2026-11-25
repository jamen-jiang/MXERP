using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.SpuCompanies;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Spu_Company表映射
/// </summary>
public sealed class SpuCompanyConfiguration : IEntityTypeConfiguration<SpuCompany>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SpuCompany> builder)
    {
        builder.ToTable("Spu_Company");

        builder.Property(x => x.SpuId).IsRequired();
        builder.Property(x => x.CompanyId).IsRequired();

        builder.HasIndex(x => x.SpuId).HasDatabaseName("IX_Spu_Company_SpuId");
        builder.HasIndex(x => x.CompanyId).HasDatabaseName("IX_Spu_Company_CompanyId");
    }
}
