using System.ComponentModel;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单类型
/// </summary>
public enum LeaveWarehouseOrderTypeEnum
{
    /// <summary>
    /// 正常出仓
    /// </summary>
    [Description("正常出仓")]
    Normal,

    /// <summary>
    /// 借调出仓
    /// </summary>
    [Description("借调出仓")]
    Second
}
