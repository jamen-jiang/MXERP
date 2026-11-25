using Snow.Domain.Repositories;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单仓储接口
/// </summary>
public interface ILeaveStockOrderRepository : IRepository<LeaveStockOrder>
{
    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单列表</returns>
    Task<List<LeaveStockOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按出库单号查询聚合（含出库单项，含关联）
    /// </summary>
    /// <param name="number">出库单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单（可空），不存在时返回 null</returns>
    Task<LeaveStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
