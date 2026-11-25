using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.PurchasePlans;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 分配结果
/// </summary>
/// <param name="LeaveStockPlan">出库计划</param>
/// <param name="PurchasePlan">采购计划</param>
public sealed record BulkOrderItemAssignResult(LeaveStockPlan LeaveStockPlan, PurchasePlan PurchasePlan);
