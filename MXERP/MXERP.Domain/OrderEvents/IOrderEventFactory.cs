namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件工厂接口
/// </summary>
public interface IOrderEventFactory
{
    /// <summary>
    /// 创建订单事件
    /// </summary>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="template">模版</param>
    /// <param name="remark">备注</param>
    /// <param name="departmentIds">部门Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>订单事件聚合</returns>
    Task<OrderEvent> CreateAsync(
        string orderType,
        string orderStatus,
        string? to,
        string? cc,
        string subject,
        string template,
        string? remark,
        IReadOnlyCollection<long>? departmentIds,
        CancellationToken cancellationToken = default);
}
