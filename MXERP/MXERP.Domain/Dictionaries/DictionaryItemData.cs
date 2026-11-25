namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典项数据(领域层内部载体)
/// </summary>
/// <param name="Value">字典值</param>
/// <param name="Label">字典标签</param>
/// <param name="Sort">排序</param>
/// <param name="Remark">备注</param>
public sealed record DictionaryItemData(string Value, string Label, int? Sort, string? Remark);
