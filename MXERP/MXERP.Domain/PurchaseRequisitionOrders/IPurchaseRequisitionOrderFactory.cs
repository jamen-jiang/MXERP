using System.Threading;
using System.Threading.Tasks;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单工厂接口
/// </summary>
public interface IPurchaseRequisitionOrderFactory
{
    /// <summary>
    /// 创建申购单
    /// </summary>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>申购单聚合</returns>
    Task<PurchaseRequisitionOrder> CreateAsync(long departmentId, long companyId, string? remark, CancellationToken cancellationToken = default);
}
