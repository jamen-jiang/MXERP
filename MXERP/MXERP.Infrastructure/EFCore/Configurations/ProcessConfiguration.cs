using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Processes;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Process表映射
/// </summary>
public sealed class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("Process");

        builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.Cost).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.IsCFUsable).IsRequired();
        builder.Property(x => x.IsKCUsable).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Type).HasDatabaseName("IX_Process_Type");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_Process_Code");
        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Process_Name");
        builder.HasIndex(x => new { x.Type, x.Code }).IsUnique().HasDatabaseName("UX_Process_Type_Code").HasFilter("[IsDeleted] = 0");
    }
}
