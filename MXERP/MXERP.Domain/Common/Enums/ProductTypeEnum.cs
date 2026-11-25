namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 产品类型枚举
/// </summary>
public static class ProductTypeEnum
{
    /// <summary>
    /// 成品
    /// </summary>
    public const string EndProduct = "EndProduct";

    /// <summary>
    /// 半成品
    /// </summary>
    public const string SemiProduct = "SemiProduct";

    /// <summary>
    /// 辅料
    /// </summary>
    public const string Accessory = "Accessory";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        EndProduct,
        SemiProduct,
        Accessory
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>是否有效</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
