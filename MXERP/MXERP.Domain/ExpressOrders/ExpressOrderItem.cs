using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单项
/// </summary>
public class ExpressOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 快递单Id
    /// </summary>
    public long? ExpressOrderId { get; private set; }
    /// <summary>
    /// 格口
    /// </summary>
    public string Grid { get; private set; } = string.Empty;
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 客户PO号
    /// </summary>
    public string CustomerPONumber { get; private set; } = string.Empty;
    /// <summary>
    /// 镜种名称
    /// </summary>
    public string LensName { get; private set; } = string.Empty;
    /// <summary>
    /// 数量
    /// </summary>
    public int Qty { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected ExpressOrderItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="grid">格口</param>
    /// <param name="orderNumber">订单号</param>
    /// <param name="customerPONumber">客户PO号</param>
    /// <param name="lensName">镜种名称</param>
    /// <param name="qty">数量</param>
    internal ExpressOrderItem(string grid, string orderNumber, string customerPONumber, string lensName, int qty)
    {
        Grid = grid;
        OrderNumber = orderNumber;
        CustomerPONumber = customerPONumber;
        LensName = lensName;
        Qty = qty;
    }

    /// <summary>
    /// 添加数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddQty(int qty)
    {
        Qty += qty;
    }
}
