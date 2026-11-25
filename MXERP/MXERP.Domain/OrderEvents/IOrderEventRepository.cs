using Snow.Domain.Repositories;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件仓储接口
/// </summary>
public interface IOrderEventRepository : IRepository<OrderEvent>
{
    /// <summary>
    /// 按订单类型、订单状态查询订单事件
    /// </summary>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>订单事件，不存在返回空</returns>
    Task<OrderEvent?> GetByOrderTypeAndOrderStatusAsync(string orderType, string orderStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断订单类型、订单状态是否存在（排除指定主键）
    /// </summary>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="id">主键，为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByOrderTypeAndOrderStatusExcludeIdAsync(string orderType, string orderStatus, long? id = null, CancellationToken cancellationToken = default);
}
