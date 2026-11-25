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
    public virtual ICollection<ExpressOrderItem> ExpressOrderItems { get; private set; } = new List<ExpressOrderItem>();

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected ExpressOrder()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="number">快递单号</param>
    /// <param name="customerCode">客户代码</param>
    public ExpressOrder(string number, string customerCode)
    {
        Number = number;
        CustomerCode = customerCode;
    }

    /// <summary>
    /// 获取快递单项列表（只读）
    /// </summary>
    /// <returns>只读视图</returns>
    public IReadOnlyList<ExpressOrderItem> GetExpressOrderItems()
    {
        return ExpressOrderItems.ToList().AsReadOnly();
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
        ExpressOrderItem? item = ExpressOrderItems.FirstOrDefault(x => x.Grid == grid && x.OrderNumber == orderNumber && x.CustomerPONumber == customerPONumber && x.LensName == lensName);
        if (item is null)
        {
            ExpressOrderItems.Add(new ExpressOrderItem(grid, orderNumber, customerPONumber, lensName, qty));
            return;
        }

        item.AddQty(qty);
    }
}
