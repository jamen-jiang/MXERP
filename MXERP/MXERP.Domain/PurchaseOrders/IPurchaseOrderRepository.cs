using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Snow.Domain.Repositories;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单仓储接口
/// </summary>
public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购单列表</returns>
    Task<List<PurchaseOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按采购单号查询聚合(含采购单项与关联)
    /// </summary>
    /// <param name="number">采购单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购单, 不存在返回 null</returns>
    Task<PurchaseOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
