using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件
/// </summary>
public class OrderEvent : AuditedAggregateRoot
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public string OrderType { get; private set; } = string.Empty;

    /// <summary>
    /// 订单状态
    /// </summary>
    public string OrderStatus { get; private set; } = string.Empty;

    /// <summary>
    /// 指定收件人
    /// </summary>
    public string? To { get; private set; }

    /// <summary>
    /// 指定抄送人
    /// </summary>
    public string? Cc { get; private set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>
    /// 模版
    /// </summary>
    public string Template { get; private set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 订单事件部门列表
    /// </summary>
    private readonly List<OrderEventDepartment> _orderEventDepartments;

    /// <summary>
    /// 订单事件部门列表
    /// </summary>
    public IReadOnlyCollection<OrderEventDepartment> OrderEventDepartments
    {
        get { return _orderEventDepartments.AsReadOnly(); }
    }

    /// <summary>
    /// 订单事件项列表
    /// </summary>
    private readonly List<OrderEventItem> _orderEventItems;

    /// <summary>
    /// 订单事件项列表
    /// </summary>
    public IReadOnlyCollection<OrderEventItem> OrderEventItems
    {
        get { return _orderEventItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器
    /// </summary>
    protected OrderEvent()
    {
        _orderEventDepartments = new List<OrderEventDepartment>();
        _orderEventItems = new List<OrderEventItem>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="template">模版</param>
    /// <param name="remark">备注</param>
    /// <param name="departmentIds">组织机构Id列表</param>
    internal OrderEvent(
        string orderType,
        string orderStatus,
        string? to,
        string? cc,
        string subject,
        string template,
        string? remark,
        IReadOnlyCollection<long>? departmentIds)
        : this()
    {
        OrderType = orderType.Trim();
        OrderStatus = orderStatus.Trim();
        To = to;
        Cc = cc;
        Subject = subject;
        Template = template;
        Remark = remark;
        SyncDepartments(departmentIds);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="orderType">订单类型</param>
    /// <param name="orderStatus">订单状态</param>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="template">模版</param>
    /// <param name="remark">备注</param>
    /// <param name="departmentIds">组织机构Id列表</param>
    internal void Modify(
        string orderType,
        string orderStatus,
        string? to,
        string? cc,
        string subject,
        string template,
        string? remark,
        IReadOnlyCollection<long>? departmentIds)
    {
        OrderType = orderType.Trim();
        OrderStatus = orderStatus.Trim();
        To = to;
        Cc = cc;
        Subject = subject;
        Template = template;
        Remark = remark;
        SyncDepartments(departmentIds);
    }

    /// <summary>
    /// 添加订单事件项
    /// </summary>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="remark">备注</param>
    /// <param name="companyIds">公司Id列表</param>
    internal void AddItem(string? to, string? cc, string? remark, IReadOnlyCollection<long>? companyIds)
    {
        _orderEventItems.Add(new OrderEventItem(to, cc, remark, companyIds));
    }

    /// <summary>
    /// 修改订单事件项
    /// </summary>
    /// <param name="itemId">订单事件项Id</param>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="remark">备注</param>
    /// <param name="companyIds">公司Id列表</param>
    internal void ModifyItem(long itemId, string? to, string? cc, string? remark, IReadOnlyCollection<long>? companyIds)
    {
        OrderEventItem? item = _orderEventItems.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);
        if (item != null)
        {
            item.Modify(to, cc, remark, companyIds);
        }
    }

    /// <summary>
    /// 删除订单事件项
    /// </summary>
    /// <param name="orderEventItemId">订单事件项Id</param>
    internal void DeleteItem(long orderEventItemId)
    {
        OrderEventItem? item = _orderEventItems.FirstOrDefault(x => x.Id == orderEventItemId && !x.IsDeleted);
        if (item != null)
        {
            item.Delete();
        }
    }

    /// <summary>
    /// 删除订单事件部门
    /// </summary>
    /// <param name="orderEventDepartmentId">订单事件部门Id</param>
    internal void DeleteDepartment(long orderEventDepartmentId)
    {
        OrderEventDepartment? department = _orderEventDepartments.FirstOrDefault(x => x.Id == orderEventDepartmentId && !x.IsDeleted);
        if (department != null)
        {
            department.Delete();
        }
    }

    /// <summary>
    /// 同步部门列表
    /// </summary>
    /// <param name="departmentIds">组织机构Id列表</param>
    private void SyncDepartments(IReadOnlyCollection<long>? departmentIds)
    {
        IReadOnlyCollection<long> target = departmentIds ?? Array.Empty<long>();

        List<OrderEventDepartment> toRemoves = _orderEventDepartments
            .Where(a => !target.Contains(a.DepartmentId))
            .ToList();
        foreach (OrderEventDepartment item in toRemoves)
        {
            DeleteDepartment(item.Id);
        }

        foreach (long departmentId in target)
        {
            if (!_orderEventDepartments.Any(x => x.DepartmentId == departmentId))
            {
                _orderEventDepartments.Add(new OrderEventDepartment(departmentId));
            }
        }
    }
}
