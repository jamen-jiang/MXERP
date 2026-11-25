namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件领域服务接口
/// </summary>
public interface IOrderEventDomainService
{
    /// <summary>
    /// 修改订单事件
    /// </summary>
    /// <param name="id">订单事件Id</param>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="template">模版</param>
    /// <param name="remark">备注</param>
    /// <param name="departmentIds">组织机构Id列表</param>
    /// <param name="orderEventItems">订单事件项数据列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>订单事件聚合</returns>
    Task<OrderEvent> ModifyAsync(
        long id,
        string orderType,
        string orderStatus,
        string? to,
        string? cc,
        string subject,
        string template,
        string? remark,
        IReadOnlyCollection<long>? departmentIds,
        IReadOnlyCollection<OrderEventItemData>? orderEventItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步订单事件项列表
    /// </summary>
    /// <param name="orderEvent">订单事件</param>
    /// <param name="orderEventItems">订单事件项数据列表</param>
    void SyncOrderEventItems(OrderEvent orderEvent, IReadOnlyCollection<OrderEventItemData>? orderEventItems);
}
