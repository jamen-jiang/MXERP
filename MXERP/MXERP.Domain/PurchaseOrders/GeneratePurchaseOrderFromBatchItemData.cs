using System.Collections.Generic;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 从采购批次 + 产品生成采购订单数据
/// </summary>
public class GeneratePurchaseOrderFromBatchItemData
{
    /// <summary>
    /// 采购批次Id
    /// </summary>
    public long PurchaseBatchId { get; }
    /// <summary>
    /// 采购批次产品Id
    /// </summary>
    public long SpuId { get; }
    /// <summary>
    /// 列表
    /// </summary>
    public IReadOnlyList<PurchaseOrderItemData> SkuInfos { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseBatchId">采购批次Id</param>
    /// <param name="spuId">采购批次产品Id</param>
    /// <param name="skuInfos">列表</param>
    public GeneratePurchaseOrderFromBatchItemData(long purchaseBatchId, long spuId, IReadOnlyList<PurchaseOrderItemData> skuInfos)
    {
        PurchaseBatchId = purchaseBatchId;
        SpuId = spuId;
        SkuInfos = skuInfos;
    }
}
