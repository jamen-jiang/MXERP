using Snow.Domain.Events;

namespace MXERP.Domain.Skus;

/// <summary>
/// SKU删除事件
/// </summary>
public class SkuDeletedEvent : DomainEvent
{
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; }

    /// <summary>
    /// 关键属性值
    /// </summary>
    public SortedDictionary<string, object?>? KeyAttributes { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="sku">SKU</param>
    public SkuDeletedEvent(Sku sku)
        : base(sku)
    {
        SpuId = sku.SpuId;
        KeyAttributes = sku.KeyAttributes;
    }
}
