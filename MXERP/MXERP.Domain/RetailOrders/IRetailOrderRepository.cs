using Snow.Domain.Repositories;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单仓储接口
/// </summary>
public interface IRetailOrderRepository : IRepository<RetailOrder>
{
    /// <summary>
    /// 按Id列表查询
    /// </summary>
    Task<List<RetailOrder>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按散镜订单号查询聚合(含订单项/跟踪)
    /// </summary>
    Task<RetailOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按客户单号查询聚合(含订单项/跟踪)
    /// </summary>
    Task<RetailOrder?> GetByCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按Id查询聚合(含订单项/跟踪)
    /// </summary>
    Task<RetailOrder?> GetWithDetailsAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断客户单号+客户PO号是否存在(不含已取消, 排除指定主键)
    /// </summary>
    Task<bool> IsExistByCustomerOrderNumberAndCustomerPONumberExcludeIdAsync(string customerOrderNumber, string? customerPONumber, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断客户PO号是否存在(不含已取消, 排除指定主键)
    /// </summary>
    Task<bool> IsExistByCustomerPONumberExcludeIdAsync(string customerPONumber, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询未同步到 HkoTracking 的散镜订单
    /// </summary>
    Task<List<RetailOrder>> GetUnsyncedListAsync(int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询未同步到 HkoTracking 的跟踪记录
    /// </summary>
    Task<List<RetailOrderTracking>> GetUnsyncedTrackingListAsync(int limit, CancellationToken cancellationToken = default);
}
