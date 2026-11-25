using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单领域服务接口
/// </summary>
public interface IBulkOrderDomainService
{
    /// <summary>
    /// 创建批量订单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="orderRef">公司订单编号(可空)</param>
    /// <param name="customerSerialNumber">客户流水号(可空)</param>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单</returns>
    Task<BulkOrder> CreateAsync(ProductTypeEnum productType, long storeId, long companyId, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改批量订单(按Id查询后修改)
    /// </summary>
    /// <param name="id">批量订单Id</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="orderRef">公司订单编号(可空)</param>
    /// <param name="customerSerialNumber">客户流水号(可空)</param>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单</returns>
    Task<BulkOrder> ModifyAsync(long id, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改批量订单(使用已有聚合, 避免重复查询)
    /// </summary>
    /// <param name="bulkOrder">批量订单</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="orderRef">公司订单编号(可空)</param>
    /// <param name="customerSerialNumber">客户流水号(可空)</param>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task ModifyAsync(BulkOrder bulkOrder, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="id">批量订单Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="name">用户名称</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单</returns>
    Task<BulkOrder> CancelAsync(long id, long userId, string name, string? remark, CancellationToken cancellationToken = default);
}
