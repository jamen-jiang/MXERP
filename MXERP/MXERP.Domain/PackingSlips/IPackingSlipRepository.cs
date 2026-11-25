using Snow.Domain.Repositories;

namespace MXERP.Domain.PackingSlips;

/// <summary>
/// 装箱单仓储接口
/// </summary>
public interface IPackingSlipRepository : IRepository<PackingSlip>
{
    /// <summary>
    /// 获取当天最大箱号
    /// </summary>
    /// <param name="storeId">门店Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>当天最大箱号，无记录返回空</returns>
    Task<int?> GetMaxCartonNoAsync(long storeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按客户单号查询
    /// </summary>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单，不存在返回空</returns>
    Task<PackingSlip?> GetByCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按品牌客户单号查询
    /// </summary>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单，不存在返回空</returns>
    Task<PackingSlip?> GetByBrandCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按订单号查询
    /// </summary>
    /// <param name="orderNumber">订单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单，不存在返回空</returns>
    Task<PackingSlip?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断客户单号是否存在
    /// </summary>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default);
}
