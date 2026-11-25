namespace MXERP.Domain.Attributes;

/// <summary>
/// 数据类型
/// </summary>
public static class DataTypeEnum
{
    /// <summary>
    /// 字符串
    /// </summary>
    public const string String = "String";

    /// <summary>
    /// 整数
    /// </summary>
    public const string Integer = "Integer";

    /// <summary>
    /// 小数
    /// </summary>
    public const string Decimal = "Decimal";

    /// <summary>
    /// 布尔值
    /// </summary>
    public const string Boolean = "Boolean";

    /// <summary>
    /// 日期
    /// </summary>
    public const string DateTime = "DateTime";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        String,
        Integer,
        Decimal,
        Boolean,
        DateTime
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
