using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典项
/// </summary>
public class DictionaryItem : DeletionAuditedEntity
{
    /// <summary>
    /// 字典Id
    /// </summary>
    public long? DictionaryId { get; private set; }

    /// <summary>
    /// 字典值
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// 字典标签
    /// </summary>
    public string Label { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected DictionaryItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="value">字典值</param>
    /// <param name="label">字典标签</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal DictionaryItem(string value, string label, int? sort, string? remark)
    {
        Value = value.Trim();
        Label = label.Trim();
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="value">字典值</param>
    /// <param name="label">字典标签</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal void Modify(string value, string label, int? sort, string? remark)
    {
        Value = value.Trim();
        Label = label.Trim();
        Sort = sort;
        Remark = remark;
    }
}
