namespace MXERP.Domain.Attributes;

/// <summary>
/// 输入类型
/// </summary>
public static class InputTypeEnum
{
    /// <summary>
    /// 文本输入框
    /// </summary>
    public const string Text = "Text";

    /// <summary>
    /// 数字输入框
    /// </summary>
    public const string Number = "Number";

    /// <summary>
    /// 日期选择框
    /// </summary>
    public const string DateTime = "DateTime";

    /// <summary>
    /// 下拉单选
    /// </summary>
    public const string Select = "Select";

    /// <summary>
    /// 下拉多选
    /// </summary>
    public const string MultiSelect = "MultiSelect";

    /// <summary>
    /// 复选框
    /// </summary>
    public const string Checkbox = "Checkbox";

    /// <summary>
    /// 单选框
    /// </summary>
    public const string Radio = "Radio";

    /// <summary>
    /// 开关
    /// </summary>
    public const string Switch = "Switch";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Text,
        Number,
        DateTime,
        Select,
        MultiSelect,
        Checkbox,
        Radio,
        Switch
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
