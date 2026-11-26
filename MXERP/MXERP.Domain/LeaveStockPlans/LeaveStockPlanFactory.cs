using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划工厂
/// </summary>
[ScopedService]
public class LeaveStockPlanFactory : ILeaveStockPlanFactory
{
    private readonly ILeaveStockPlanRepository _leaveStockPlanRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockPlanRepository">出库计划仓储</param>
    public LeaveStockPlanFactory(ILeaveStockPlanRepository leaveStockPlanRepository)
    {
        _leaveStockPlanRepository = leaveStockPlanRepository;
    }

    /// <inheritdoc />
    public async Task<LeaveStockPlan> CreateAsync(string productType, long storeId, long companyId, string relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default)
    {
        bool isExist = await _leaveStockPlanRepository.IsExistByRelationAsync(relationType, relationId, relationItemId, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("出库计划已存在!");
        }

        return new LeaveStockPlan(productType, storeId, companyId, relationType, relationId, relationItemId, relationNumber, relationCustomerOrderNumber, relationCustomerPONumber, relationOrderRef, remark);
    }

    /// <inheritdoc />
    public async Task<LeaveStockPlan> CreateAsync(string productType, long departmentId, long companyId, string relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default)
    {
        bool isExist = await _leaveStockPlanRepository.IsExistByRelationAsync(relationType, relationId, null, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("出库计划已存在!");
        }

        return new LeaveStockPlan(productType, departmentId, companyId, relationType, relationId, relationNumber, remark);
    }
}
