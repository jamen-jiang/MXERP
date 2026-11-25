using Snow.Domain.Repositories;

namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项仓储接口
/// </summary>
public interface ISalesSpuOtherAssignItemRepository : IRepository<SalesSpuOtherAssignItem>
{
    /// <summary>
    /// 按销售产品Id + 焗色查询
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品其他分配项, 不存在返回 null</returns>
    Task<SalesSpuOtherAssignItem?> GetBySalesSpuIdAndCoatingAsync(long salesSpuId, string coating, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断销售产品Id + 焗色是否存在(排除指定主键)
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistBySalesSpuIdAndCoatingExcludeIdAsync(long salesSpuId, string coating, long? id = null, CancellationToken cancellationToken = default);
}
