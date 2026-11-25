using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次仓储接口
/// </summary>
public interface IPurchaseBatchRepository : IRepository<PurchaseBatch>
{
    /// <summary>
    /// 按Id列表查询采购批次（含采购批次项）
    /// </summary>
    /// <param name="ids">主键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次列表（ids 为空时返回空列表）</returns>
    Task<List<PurchaseBatch>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按关联类型 + 关联Id查询列表（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次列表</returns>
    Task<List<PurchaseBatch>> GetListByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断关联类型 + 关联Id是否存在（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断关联类型 + 关联Id + 关联项Id是否存在（不含已取消）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long relationItemId, CancellationToken cancellationToken = default);
}
