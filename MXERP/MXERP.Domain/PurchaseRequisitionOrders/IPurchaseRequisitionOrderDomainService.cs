using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.Common.Contracts;
using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.PurchasePlans;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单领域服务接口
/// </summary>
public interface IPurchaseRequisitionOrderDomainService
{
    /// <summary>
    /// 同步申购单项列表
    /// </summary>
    /// <param name="purchaseRequisitionOrder">申购单聚合</param>
    /// <param name="purchaseRequisitionOrderItems">申购单项数据列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncPurchaseRequisitionOrderItemsAsync(PurchaseRequisitionOrder purchaseRequisitionOrder, IReadOnlyCollection<PurchaseRequisitionOrderItemData>? purchaseRequisitionOrderItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="id">申购单Id</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>申购单</returns>
    Task<PurchaseRequisitionOrder> ModifyAsync(long id, long departmentId, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 自动分配
    /// </summary>
    /// <param name="id">申购单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库计划 + 采购计划</returns>
    Task<PurchaseRequisitionOrderAssignResult> AutomaticAssignAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 全订购
    /// </summary>
    /// <param name="id">申购单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库计划 + 采购计划</returns>
    Task<PurchaseRequisitionOrderAssignResult> AllCustomizedAssignAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配申购单
    /// </summary>
    /// <param name="id">申购单Id</param>
    /// <param name="leaveStockPlanSkuInfos">出库计划SKU信息列表</param>
    /// <param name="purchasePlanSkuInfos">采购计划SKU信息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库计划 + 采购计划</returns>
    Task<PurchaseRequisitionOrderAssignResult> AssignAsync(long id, IReadOnlyCollection<SkuInfoData> leaveStockPlanSkuInfos, IReadOnlyCollection<SkuInfoData> purchasePlanSkuInfos, CancellationToken cancellationToken = default);
}
