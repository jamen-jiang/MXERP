namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性项数据
/// </summary>
/// <param name="Value">属性值</param>
/// <param name="Name">属性值名称</param>
/// <param name="IsDefault">是否默认</param>
/// <param name="Sort">排序</param>
public sealed record AttributeItemData(string Value, string Name, bool IsDefault, int? Sort);
