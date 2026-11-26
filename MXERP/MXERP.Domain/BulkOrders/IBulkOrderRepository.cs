using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单仓储接口
/// </summary>
public interface IBulkOrderRepository : IRepository<BulkOrder>
{
    /// <summary>
    /// 按公司订单编号查询
    /// </summary>
    /// <param name="number">公司订单编号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单(可空), 不存在返回 null</returns>
    Task<BulkOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断客户单号 + 客户PO号 + 产品类型是否存在
    /// </summary>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="productType">产品类型</param>
    /// <param name="id">排除的主键Id(可空), 修改时排除自身</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCustomerOrderNumberAndCustomerPONumberAndProductTypeExcludeIdAsync(string customerOrderNumber, string? customerPONumber, string productType, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断客户PO号 + 产品类型是否存在
    /// </summary>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="productType">产品类型</param>
    /// <param name="id">排除的主键Id(可空), 修改时排除自身</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCustomerPONumberAndProductTypeExcludeIdAsync(string customerPONumber, string productType, long? id = null, CancellationToken cancellationToken = default);
}
