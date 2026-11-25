using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.PurchasePlans;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单分配结果（出库计划 + 采购计划）
/// </summary>
/// <param name="LeaveStockPlan">出库计划</param>
/// <param name="PurchasePlan">采购计划</param>
public sealed record PurchaseRequisitionOrderAssignResult(LeaveStockPlan LeaveStockPlan, PurchasePlan PurchasePlan);
