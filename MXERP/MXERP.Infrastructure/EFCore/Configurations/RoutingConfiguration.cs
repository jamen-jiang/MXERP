using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.Routings;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// Routing表映射
/// </summary>
public sealed class RoutingConfiguration : IEntityTypeConfiguration<Routing>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Routing> builder)
    {
        builder.ToTable("Routing");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Disabled).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(500);

        builder.HasIndex(x => x.Name).HasDatabaseName("IX_Routing_Name");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Routing_Code").HasFilter("[IsDeleted] = 0");

        builder.HasMany(x => x.RoutingSteps)
            .WithOne()
            .HasForeignKey(x => x.RoutingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
