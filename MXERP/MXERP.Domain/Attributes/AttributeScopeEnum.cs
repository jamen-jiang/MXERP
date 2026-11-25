namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性范围
/// </summary>
public static class AttributeScopeEnum
{
    /// <summary>
    /// 产品
    /// </summary>
    public const string Spu = "Spu";

    /// <summary>
    /// Sku
    /// </summary>
    public const string Sku = "Sku";

    /// <summary>
    /// 销售产品
    /// </summary>
    public const string SalesSpu = "SalesSpu";

    /// <summary>
    /// 销售Sku
    /// </summary>
    public const string SalesSku = "SalesSku";

    /// <summary>
    /// 公用
    /// </summary>
    public const string Common = "Common";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Spu,
        Sku,
        SalesSpu,
        SalesSku,
        Common
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
