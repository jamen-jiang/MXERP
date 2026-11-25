using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次工厂接口
/// </summary>
public interface ILeaveStockBatchFactory
{
    /// <summary>
    /// 创建出库批次（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="leaveStockPlanId">出库计划Id（可空）</param>
    /// <param name="purchaseBatchId">采购批次Id（可空）</param>
    /// <param name="purchaseBatchNumber">采购批次号（可空）</param>
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
    /// <returns>出库批次</returns>
    Task<LeaveStockBatch> CreateAsync(ProductTypeEnum productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long storeId, long companyId, BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建出库批次（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="leaveStockPlanId">出库计划Id（可空）</param>
    /// <param name="purchaseBatchId">采购批次Id（可空）</param>
    /// <param name="purchaseBatchNumber">采购批次号（可空）</param>
    /// <param name="departmentId">申购部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次</returns>
    Task<LeaveStockBatch> CreateAsync(ProductTypeEnum productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long departmentId, long companyId, BusinessOrderTypeEnum relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default);
}
