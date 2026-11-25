using Snow.Domain.Repositories;

namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单仓储接口
/// </summary>
public interface IExpressOrderRepository : IRepository<ExpressOrder>
{
    /// <summary>
    /// 按快递单号查询, 不存在返回 null
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>快递单, 不存在返回 null</returns>
    Task<ExpressOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断快递单号是否存在(排除指定主键)
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="id">排除的主键Id, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByNumberExcludeIdAsync(string number, long? id = null, CancellationToken cancellationToken = default);
}
