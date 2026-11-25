namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次工厂接口
/// </summary>
public interface IPurchaseBatchFactory
{
    /// <summary>
    /// 创建采购批次（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="purchasePlanId">采购计划Id（可空）</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="relationNumber">关联单号</param>
    /// <param name="relationCustomerOrderNumber">关联客户单号（可空）</param>
    /// <param name="relationCustomerPONumber">关联客户PO号（可空）</param>
    /// <param name="relationOrderRef">关联公司订单编号（可空）</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次</returns>
    Task<PurchaseBatch> CreateAsync(string productType, long? purchasePlanId, long storeId, long companyId, string relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建采购批次（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="purchasePlanId">采购计划Id（可空）</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联单号</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次</returns>
    Task<PurchaseBatch> CreateAsync(string productType, long? purchasePlanId, long departmentId, long companyId, string relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default);
}
