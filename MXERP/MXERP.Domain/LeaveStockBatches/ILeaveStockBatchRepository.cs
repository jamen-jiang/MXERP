using Snow.Domain.Repositories;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次仓储接口
/// </summary>
public interface ILeaveStockBatchRepository : IRepository<LeaveStockBatch>
{
    /// <summary>
    /// 按主键集合批量获取出库批次列表
    /// </summary>
    /// <param name="ids">主键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次列表</returns>
    Task<List<LeaveStockBatch>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按批次号查询（不含已取消）, 不存在返回 null
    /// </summary>
    /// <param name="number">批次号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次, 不存在返回 null</returns>
    Task<LeaveStockBatch?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按关联类型 + 关联Id查询列表（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次列表</returns>
    Task<List<LeaveStockBatch>> GetListByRelationAsync(string relationType, long relationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按采购批次Id查询列表（不含已取消）
    /// </summary>
    /// <param name="purchaseBatchId">采购批次Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次列表</returns>
    Task<List<LeaveStockBatch>> GetListByPurchaseBatchIdAsync(long purchaseBatchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断关联类型 + 关联Id是否存在（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByRelationAsync(string relationType, long relationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断关联类型 + 关联Id + 关联项Id是否存在（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByRelationAsync(string relationType, long relationId, long relationItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断采购批次Id是否存在（不含已取消）
    /// </summary>
    /// <param name="purchaseBatchId">采购批次Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByPurchaseBatchIdAsync(long purchaseBatchId, CancellationToken cancellationToken = default);
}
