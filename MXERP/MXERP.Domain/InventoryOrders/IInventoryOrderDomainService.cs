using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单领域服务接口
/// </summary>
public interface IInventoryOrderDomainService
{
    /// <summary>
    /// 同步盘点单项列表
    /// </summary>
    /// <param name="inventoryOrder">盘点单聚合</param>
    /// <param name="skuInfos">SKU信息列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncInventoryOrderItemsAsync(InventoryOrder inventoryOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default);

    /// <summary>
    /// 审核盘点单
    /// </summary>
    /// <param name="id">盘点单Id</param>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>盘点单聚合</returns>
    Task<InventoryOrder> CheckAsync(long id, long userId, string name, CancellationToken cancellationToken = default);
}
