using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单
/// </summary>
public class ExpressOrder : AuditedAggregateRoot
{
    /// <summary>
    /// 快递单号
    /// </summary>
    public string Number { get; private set; } = string.Empty;
    /// <summary>
    /// 客户代码
    /// </summary>
    public string CustomerCode { get; private set; } = string.Empty;

    /// <summary>
    /// 快递单项
    /// </summary>
    private readonly List<ExpressOrderItem> _expressOrderItems;

    /// <summary>
    /// 快递单项
    /// </summary>
    public IReadOnlyCollection<ExpressOrderItem> ExpressOrderItems
    {
        get { return _expressOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected ExpressOrder()
    {
        _expressOrderItems = new List<ExpressOrderItem>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="customerCode">客户代码</param>
    public ExpressOrder(string number, string customerCode)
        : this()
    {
        Number = number;
        CustomerCode = customerCode;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="customerCode">客户代码</param>
    public void Modify(string customerCode)
    {
        CustomerCode = customerCode;
    }

    /// <summary>
    /// 添加或修改项
    /// </summary>
    /// <param name="grid">格口</param>
    /// <param name="orderNumber">订单号</param>
    /// <param name="customerPONumber">客户PO号</param>
    /// <param name="lensName">镜种名称</param>
    /// <param name="qty">数量</param>
    public void AddOrModifyItem(string grid, string orderNumber, string customerPONumber, string lensName, int qty)
    {
        ExpressOrderItem? item = _expressOrderItems.FirstOrDefault(x => x.Grid == grid && x.OrderNumber == orderNumber && x.CustomerPONumber == customerPONumber && x.LensName == lensName);
        if (item is null)
        {
            _expressOrderItems.Add(new ExpressOrderItem(grid, orderNumber, customerPONumber, lensName, qty));
            return;
        }

        item.AddQty(qty);
    }
}
