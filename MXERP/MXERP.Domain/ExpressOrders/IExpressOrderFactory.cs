namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单工厂接口
/// </summary>
public interface IExpressOrderFactory
{
    /// <summary>
    /// 创建快递单
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="customerCode">客户代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>快递单</returns>
    Task<ExpressOrder> CreateAsync(string number, string customerCode, CancellationToken cancellationToken = default);
}
