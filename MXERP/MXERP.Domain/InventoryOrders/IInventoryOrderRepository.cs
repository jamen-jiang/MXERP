using System.Threading;
using System.Threading.Tasks;
using Snow.Domain.Repositories;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单仓储接口
/// </summary>
public interface IInventoryOrderRepository : IRepository<InventoryOrder>
{
    /// <summary>
    /// 按盘点单号查询聚合(含盘点单项)
    /// </summary>
    /// <param name="number">盘点单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>盘点单, 不存在返回 null</returns>
    Task<InventoryOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
