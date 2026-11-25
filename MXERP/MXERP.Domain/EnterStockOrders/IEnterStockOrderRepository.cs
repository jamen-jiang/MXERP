using Snow.Domain.Repositories;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单仓储接口
/// </summary>
public interface IEnterStockOrderRepository : IRepository<EnterStockOrder>
{
    /// <summary>
    /// 按Id查询（含入库单项与关联）
    /// </summary>
    /// <param name="id">入库单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单（可空），不存在时返回 null</returns>
    Task<EnterStockOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单列表</returns>
    Task<List<EnterStockOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按入库单号查询聚合（含入库单项，含关联）
    /// </summary>
    /// <param name="number">入库单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单（可空），不存在时返回 null</returns>
    Task<EnterStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
