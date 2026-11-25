using System.Collections.Generic;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 从采购批次生成采购订单数据
/// </summary>
public class GeneratePurchaseOrderFromBatchData
{
    /// <summary>
    /// 采购批次Id
    /// </summary>
    public long PurchaseBatchId { get; }
    /// <summary>
    /// 列表
    /// </summary>
    public IReadOnlyList<PurchaseOrderItemData> SkuInfos { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseBatchId">采购批次Id</param>
    /// <param name="skuInfos">列表</param>
    public GeneratePurchaseOrderFromBatchData(long purchaseBatchId, IReadOnlyList<PurchaseOrderItemData> skuInfos)
    {
        PurchaseBatchId = purchaseBatchId;
        SkuInfos = skuInfos;
    }
}
