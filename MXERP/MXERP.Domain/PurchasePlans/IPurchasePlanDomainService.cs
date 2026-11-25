namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划领域服务接口
/// </summary>
public interface IPurchasePlanDomainService
{
    /// <summary>
    /// 同步采购计划项列表
    /// </summary>
    /// <param name="purchasePlan">采购计划</param>
    /// <param name="skuInfos">采购计划项数据列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncPurchasePlanItemsAsync(PurchasePlan purchasePlan, IReadOnlyCollection<PurchasePlanItemData>? skuInfos, CancellationToken cancellationToken = default);
}
