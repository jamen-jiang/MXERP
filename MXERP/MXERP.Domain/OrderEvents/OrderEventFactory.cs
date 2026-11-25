using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件工厂
/// </summary>
[ScopedService]
public class OrderEventFactory : IOrderEventFactory
{
    private readonly IOrderEventRepository _orderEventRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="orderEventRepository">订单事件仓储</param>
    public OrderEventFactory(IOrderEventRepository orderEventRepository)
    {
        _orderEventRepository = orderEventRepository;
    }

    /// <inheritdoc />
    public async Task<OrderEvent> CreateAsync(
        string orderType,
        string orderStatus,
        string? to,
        string? cc,
        string subject,
        string template,
        string? remark,
        IReadOnlyCollection<long>? departmentIds,
        CancellationToken cancellationToken = default)
    {
        if (await _orderEventRepository.IsExistByOrderTypeAndOrderStatusExcludeIdAsync(orderType, orderStatus, null, cancellationToken))
        {
            throw new UserFriendlyException("订单事件已存在!");
        }
        return new OrderEvent(orderType, orderStatus, to, cc, subject, template, remark, departmentIds);
    }
}
