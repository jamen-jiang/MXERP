using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划工厂接口
/// </summary>
public interface IPurchasePlanFactory
{
    /// <summary>
    /// 生成采购计划（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="relationCustomerOrderNumber">业务单客户单号（可空）</param>
    /// <param name="relationCustomerPONumber">业务单客户PO号（可空）</param>
    /// <param name="relationOrderRef">业务单公司订单编号（可空）</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购计划</returns>
    Task<PurchasePlan> CreateAsync(ProductTypeEnum productType, long storeId, long companyId, BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成采购计划（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购计划</returns>
    Task<PurchasePlan> CreateAsync(ProductTypeEnum productType, long departmentId, long companyId, BusinessOrderTypeEnum relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default);
}
