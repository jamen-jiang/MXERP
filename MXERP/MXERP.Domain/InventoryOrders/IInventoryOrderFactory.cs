using MXERP.Domain.Common.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单工厂接口
/// </summary>
public interface IInventoryOrderFactory
{
    /// <summary>
    /// 创建盘点单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>盘点单聚合</returns>
    Task<InventoryOrder> CreateAsync(string productType, string? remark, CancellationToken cancellationToken = default);
}
