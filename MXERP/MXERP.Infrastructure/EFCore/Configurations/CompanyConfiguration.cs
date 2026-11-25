using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Companies;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Company表映射(对应 Java 的 @Table/@Column/@Index)
/// </summary>
public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");

        builder.Property(x => x.Region).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Group).HasColumnName("Group").HasMaxLength(50);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShortName).HasMaxLength(100);
        builder.Property(x => x.NumberCode).HasMaxLength(50);
        builder.Property(x => x.Telephone).HasMaxLength(100);
        builder.Property(x => x.Fax).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.AreaCode).HasMaxLength(20);
        builder.Property(x => x.PostCode).HasMaxLength(20);
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.Property(x => x.MarkUp).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(800);
        builder.Property(x => x.BillingAddress).HasMaxLength(800);
        builder.Property(x => x.Sort);
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Group).HasDatabaseName("IX_Company_Group");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Company_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Company_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Company_Code").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.CompanyContacts)
            .WithOne()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
