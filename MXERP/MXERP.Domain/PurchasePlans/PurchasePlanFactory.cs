using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划工厂
/// </summary>
[ScopedService]
public class PurchasePlanFactory : IPurchasePlanFactory
{
    /// <summary>
    /// 采购计划仓储
    /// </summary>
    private readonly IPurchasePlanRepository _purchasePlanRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchasePlanRepository">采购计划仓储</param>
    public PurchasePlanFactory(IPurchasePlanRepository purchasePlanRepository)
    {
        _purchasePlanRepository = purchasePlanRepository;
    }

    /// <inheritdoc />
    public async Task<PurchasePlan> CreateAsync(ProductTypeEnum productType, long storeId, long companyId, BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default)
    {
        bool isExist = await _purchasePlanRepository.IsExistByRelationAsync(relationType, relationId, relationItemId, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("采购计划已存在!");
        }

        return new PurchasePlan(productType, storeId, companyId, relationType, relationId, relationItemId, relationNumber, relationCustomerOrderNumber, relationCustomerPONumber, relationOrderRef, remark);
    }

    /// <inheritdoc />
    public async Task<PurchasePlan> CreateAsync(ProductTypeEnum productType, long departmentId, long companyId, BusinessOrderTypeEnum relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default)
    {
        bool isExist = await _purchasePlanRepository.IsExistByRelationAsync(relationType, relationId, null, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("采购计划已存在!");
        }

        return new PurchasePlan(productType, departmentId, companyId, relationType, relationId, relationNumber, remark);
    }
}
