namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单领域服务接口
/// </summary>
public interface IExpressOrderDomainService
{
    /// <summary>
    /// 修改快递单（按Id查询后修改）
    /// </summary>
    /// <param name="id">快递单Id</param>
    /// <param name="customerCode">客户代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>快递单</returns>
    Task<ExpressOrder> ModifyAsync(long id, string customerCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改快递单（使用已有聚合）
    /// </summary>
    /// <param name="expressOrder">快递单</param>
    /// <param name="customerCode">客户代码</param>
    void Modify(ExpressOrder expressOrder, string customerCode);

    /// <summary>
    /// 导入生成: 快递单存在则修改, 不存在则创建
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="customerCode">客户代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>快递单</returns>
    Task<ExpressOrder> ImportGenerateAsync(string number, string customerCode, CancellationToken cancellationToken = default);
}
