using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件领域服务
/// </summary>
[ScopedService]
public class OrderEventDomainService : IOrderEventDomainService
{
    private readonly IOrderEventRepository _orderEventRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="orderEventRepository">订单事件仓储</param>
    public OrderEventDomainService(IOrderEventRepository orderEventRepository)
    {
        _orderEventRepository = orderEventRepository;
    }

    /// <inheritdoc />
    public async Task<OrderEvent> ModifyAsync(
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
        CancellationToken cancellationToken = default)
    {
        if (await _orderEventRepository.IsExistByOrderTypeAndOrderStatusExcludeIdAsync(orderType, orderStatus, id, cancellationToken))
        {
            throw new UserFriendlyException("订单事件已存在!");
        }
        OrderEvent? model = await _orderEventRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("订单事件不存在!");
        }
        model.Modify(orderType, orderStatus, to, cc, subject, template, remark, departmentIds);
        SyncOrderEventItems(model, orderEventItems);
        return model;
    }

    /// <inheritdoc />
    public void SyncOrderEventItems(OrderEvent orderEvent, IReadOnlyCollection<OrderEventItemData>? orderEventItems)
    {
        IReadOnlyCollection<OrderEventItemData> target = orderEventItems ?? Array.Empty<OrderEventItemData>();

        List<OrderEventItem> toRemoves = orderEvent.OrderEventItems
            .Where(a => !target.Any(i => i.Id == a.Id))
            .ToList();
        foreach (OrderEventItem item in toRemoves)
        {
            orderEvent.DeleteItem(item.Id);
        }

        foreach (OrderEventItemData item in target)
        {
            if (item.Id != null && item.Id > 0)
            {
                orderEvent.ModifyItem(item.Id.Value, item.To, item.Cc, item.Remark, item.CompanyIds);
            }
            else
            {
                orderEvent.AddItem(item.To, item.Cc, item.Remark, item.CompanyIds);
            }
        }
    }
}
