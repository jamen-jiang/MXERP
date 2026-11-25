using Snow.Domain.Repositories;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单仓储接口
/// </summary>
public interface ILeaveWarehouseOrderRepository : IRepository<LeaveWarehouseOrder>
{
    /// <summary>
    /// 按单号查询
    /// </summary>
    /// <param name="number">单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出仓单, 不存在返回 null</returns>
    Task<LeaveWarehouseOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
