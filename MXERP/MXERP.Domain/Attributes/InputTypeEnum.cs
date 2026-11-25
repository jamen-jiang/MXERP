using System.ComponentModel;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 输入类型
/// </summary>
public enum InputTypeEnum
{
    /// <summary>
    /// 文本输入框
    /// </summary>
    [Description("文本输入框")]
    Text,

    /// <summary>
    /// 数字输入框
    /// </summary>
    [Description("数字输入框")]
    Number,

    /// <summary>
    /// 日期选择框
    /// </summary>
    [Description("日期选择框")]
    DateTime,

    /// <summary>
    /// 下拉单选
    /// </summary>
    [Description("下拉单选")]
    Select,

    /// <summary>
    /// 下拉多选
    /// </summary>
    [Description("下拉多选")]
    MultiSelect,

    /// <summary>
    /// 复选框
    /// </summary>
    [Description("复选框")]
    Checkbox,

    /// <summary>
    /// 单选框
    /// </summary>
    [Description("单选框")]
    Radio,

    /// <summary>
    /// 开关
    /// </summary>
    [Description("开关")]
    Switch
}
