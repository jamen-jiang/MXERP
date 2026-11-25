using System.Collections.Generic;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单状态
/// </summary>
public static class PurchaseOrderStatusEnum
{
    /// <summary>
    /// 待审核
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// 待入库
    /// </summary>
    public const string EnterStock = "EnterStock";

    /// <summary>
    /// 部分入库
    /// </summary>
    public const string PartialEnterStock = "PartialEnterStock";

    /// <summary>
    /// 全部入库
    /// </summary>
    public const string AllEnterStock = "AllEnterStock";

    /// <summary>
    /// 超出入库
    /// </summary>
    public const string OverEnterStock = "OverEnterStock";

    /// <summary>
    /// 已完成
    /// </summary>
    public const string Finished = "Finished";

    /// <summary>
    /// 已取消
    /// </summary>
    public const string Canceled = "Canceled";

    /// <summary>
    /// 全部状态
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Pending,
        EnterStock,
        PartialEnterStock,
        AllEnterStock,
        OverEnterStock,
        Finished,
        Canceled,
    };

    /// <summary>
    /// 判断状态编码是否有效
    /// </summary>
    /// <param name="code">状态编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return All.Contains(code);
    }
}
