using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.BulkOrderItems;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 批量订单项销售SKU行实体配置
/// </summary>
public sealed class BulkOrderItemSalesSkuConfiguration : IEntityTypeConfiguration<BulkOrderItemSalesSku>
{
    /// <summary>
    /// 配置实体
    /// </summary>
    /// <param name="builder">实体构建器</param>
    public void Configure(EntityTypeBuilder<BulkOrderItemSalesSku> builder)
    {
        builder.ToTable("BulkOrderItemRange");
        builder.Property(x => x.SkuAttributes).HasSortedDictionaryJsonConversion().HasColumnName("SkuAttributes");
        builder.Property(x => x.OtherAttributes).HasSortedDictionaryJsonConversion().HasColumnName("OtherAttributes");

        builder.HasIndex(x => x.BulkOrderItemId).HasDatabaseName("IX_BulkOrderItemRange_BulkOrderItemId");
    }
}
