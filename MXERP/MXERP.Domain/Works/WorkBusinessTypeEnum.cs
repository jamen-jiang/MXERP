namespace MXERP.Domain.Works;

/// <summary>
/// 工作业务类型
/// </summary>
public static class WorkBusinessTypeEnum
{
    /// <summary>
    /// 申购单
    /// </summary>
    public const string PurchaseRequisitionOrder = "PurchaseRequisitionOrder";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        PurchaseRequisitionOrder
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
