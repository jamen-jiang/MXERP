using Snow.Domain.Repositories;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单仓储接口
/// </summary>
public interface IExternalLeaveStockOrderRepository : IRepository<ExternalLeaveStockOrder>
{
    /// <summary>
    /// 按单号查询聚合(含跟踪), 不存在返回 null
    /// </summary>
    /// <param name="number">单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>外部出库单, 不存在返回 null</returns>
    Task<ExternalLeaveStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
