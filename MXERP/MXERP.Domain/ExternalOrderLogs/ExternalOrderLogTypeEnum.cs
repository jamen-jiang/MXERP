namespace MXERP.Domain.ExternalOrderLogs;

/// <summary>
/// 外部订单日志类型
/// </summary>
public static class ExternalOrderLogTypeEnum
{
    /// <summary>
    /// 批量订单
    /// </summary>
    public const string BulkOrder = "BulkOrder";

    /// <summary>
    /// 散镜订单
    /// </summary>
    public const string RetailOrder = "RetailOrder";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        BulkOrder,
        RetailOrder
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>true 表示有效</returns>
    public static bool IsValid(string? code)
    {
        if (code is null)
        {
            return false;
        }

        return All.Contains(code);
    }
}
