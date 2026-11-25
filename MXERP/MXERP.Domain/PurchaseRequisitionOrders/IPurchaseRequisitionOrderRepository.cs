using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Snow.Domain.Repositories;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单仓储接口
/// </summary>
public interface IPurchaseRequisitionOrderRepository : IRepository<PurchaseRequisitionOrder>
{
    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>申购单列表</returns>
    Task<List<PurchaseRequisitionOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按申购单号查询聚合（含申购单项）
    /// </summary>
    /// <param name="number">申购单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>申购单，不存在时返回 null</returns>
    Task<PurchaseRequisitionOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
}
