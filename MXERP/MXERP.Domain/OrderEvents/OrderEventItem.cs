using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件项
/// </summary>
public class OrderEventItem : DeletionAuditedEntity
{
    /// <summary>
    /// 订单事件Id
    /// </summary>
    public long? OrderEventId { get; private set; }

    /// <summary>
    /// 指定收件人
    /// </summary>
    public string? To { get; private set; }

    /// <summary>
    /// 指定抄送人
    /// </summary>
    public string? Cc { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 订单事件项公司列表
    /// </summary>
    private readonly List<OrderEventItemCompany> _orderEventItemCompanys;

    /// <summary>
    /// 订单事件项公司列表
    /// </summary>
    public IReadOnlyCollection<OrderEventItemCompany> OrderEventItemCompanys
    {
        get { return _orderEventItemCompanys.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器
    /// </summary>
    protected OrderEventItem()
    {
        _orderEventItemCompanys = new List<OrderEventItemCompany>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="remark">备注</param>
    /// <param name="companyIds">公司Id列表</param>
    internal OrderEventItem(string? to, string? cc, string? remark, IReadOnlyCollection<long>? companyIds)
        : this()
    {
        To = to;
        Cc = cc;
        Remark = remark;
        SyncCompanys(companyIds);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="to">指定收件人</param>
    /// <param name="cc">指定抄送人</param>
    /// <param name="remark">备注</param>
    /// <param name="companyIds">公司Id列表</param>
    internal void Modify(string? to, string? cc, string? remark, IReadOnlyCollection<long>? companyIds)
    {
        To = to;
        Cc = cc;
        Remark = remark;
        SyncCompanys(companyIds);
    }

    /// <summary>
    /// 删除公司
    /// </summary>
    /// <param name="orderEventItemCompanyId">订单事件项公司Id</param>
    internal void DeleteCompany(long orderEventItemCompanyId)
    {
        OrderEventItemCompany? company = _orderEventItemCompanys.FirstOrDefault(x => x.Id == orderEventItemCompanyId && !x.IsDeleted);
        if (company != null)
        {
            company.Delete();
        }
    }

    /// <summary>
    /// 同步公司列表
    /// </summary>
    /// <param name="companyIds">公司Id列表</param>
    private void SyncCompanys(IReadOnlyCollection<long>? companyIds)
    {
        IReadOnlyCollection<long> target = companyIds ?? Array.Empty<long>();

        List<OrderEventItemCompany> toRemoves = _orderEventItemCompanys
            .Where(a => !target.Contains(a.CompanyId))
            .ToList();
        foreach (OrderEventItemCompany item in toRemoves)
        {
            DeleteCompany(item.Id);
        }

        foreach (long companyId in target)
        {
            if (!_orderEventItemCompanys.Any(x => x.CompanyId == companyId))
            {
                _orderEventItemCompanys.Add(new OrderEventItemCompany(companyId));
            }
        }
    }
}
