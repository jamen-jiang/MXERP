using System.ComponentModel;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单类型
/// </summary>
public enum LeaveStockOrderTypeEnum
{
    /// <summary>
    /// 抽仓
    /// </summary>
    [Description("抽仓")]
    Deduct,

    /// <summary>
    /// 订购
    /// </summary>
    [Description("订购")]
    Customized
}
