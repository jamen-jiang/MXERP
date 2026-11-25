using System.ComponentModel;

namespace MXERP.Domain.ExternalOrderLogs;

/// <summary>
/// 外部订单日志类型
/// </summary>
public enum ExternalOrderLogTypeEnum
{
    /// <summary>
    /// 批量订单
    /// </summary>
    [Description("批量订单")]
    BulkOrder,

    /// <summary>
    /// 散镜订单
    /// </summary>
    [Description("散镜订单")]
    RetailOrder
}
