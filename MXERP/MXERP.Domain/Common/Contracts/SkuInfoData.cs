namespace MXERP.Domain.Common.Contracts;

/// <summary>
/// SKU信息数据
/// <para>用于承载SKU、仓库及数量信息。</para>
/// </summary>
public class SkuInfoData
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量（可空，为空视为0）</param>
    public SkuInfoData(long skuId, long warehouseId, double? qty)
    {
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty ?? 0;
    }

    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; }
}
