using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单工厂接口
/// </summary>
public interface ILeaveStockOrderFactory
{
    /// <summary>
    /// 创建出库单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原始Id（可空）</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单聚合</returns>
    Task<LeaveStockOrder> CreateAsync(string productType, long? originalId, string businessType, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建出库单（带关联类型）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原始Id（可空）</param>
    /// <param name="businessOrderType">业务订单类型</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单聚合</returns>
    Task<LeaveStockOrder> CreateAsync(string productType, long? originalId, string businessOrderType, string businessType, string? remark, CancellationToken cancellationToken = default);
}
