using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库批次生成出库单参数
/// </summary>
public class GenerateLeaveStockOrderFromBatchData
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="skuInfos">Sku信息列表</param>
    public GenerateLeaveStockOrderFromBatchData(long leaveStockBatchId, IReadOnlyCollection<UniqueSkuInfoData> skuInfos)
    {
        LeaveStockBatchId = leaveStockBatchId;
        SkuInfos = skuInfos;
    }

    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long LeaveStockBatchId { get; }

    /// <summary>
    /// Sku信息列表
    /// </summary>
    public IReadOnlyCollection<UniqueSkuInfoData> SkuInfos { get; }
}
