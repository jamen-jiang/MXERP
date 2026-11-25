using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典
/// </summary>
public class Dictionary : AuditedAggregateRoot
{
    /// <summary>
    /// 字典类型
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// 字典类型名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 字典值列表
    /// </summary>
    public ICollection<DictionaryItem> DictionaryItems { get; private set; } = new List<DictionaryItem>();

    /// <summary>
    /// 构造器
    /// </summary>
    protected Dictionary()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="category">字典类型</param>
    /// <param name="name">字典类型名称</param>
    /// <param name="remark">备注</param>
    internal Dictionary(string category, string name, string? remark)
    {
        Category = category.Trim();
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="category">字典类型</param>
    /// <param name="name">字典类型名称</param>
    /// <param name="remark">备注</param>
    internal void Modify(string category, string name, string? remark)
    {
        Category = category.Trim();
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 获取字典值列表(过滤已删除)
    /// </summary>
    /// <returns>字典值列表</returns>
    public IReadOnlyList<DictionaryItem> GetDictionaryItems()
    {
        return DictionaryItems.Where(x => !x.IsDeleted).ToList();
    }

    /// <summary>
    /// 新增字典项
    /// </summary>
    /// <param name="value">字典值</param>
    /// <param name="label">字典标签</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    public void AddItem(string value, string label, int? sort, string? remark)
    {
        if (IsExistItemByValue(value, null))
        {
            throw new UserFriendlyException($"字典类型{Category}字典项值{value}已存在!");
        }

        if (IsExistItemByLabel(label, null))
        {
            throw new UserFriendlyException($"字典类型{Category}字典项标签{label}已存在!");
        }

        DictionaryItems.Add(new DictionaryItem(value, label, sort, remark));
    }

    /// <summary>
    /// 修改字典项
    /// </summary>
    /// <param name="itemId">字典项Id</param>
    /// <param name="value">字典值</param>
    /// <param name="label">字典标签</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    public void ModifyItem(long itemId, string value, string label, int? sort, string? remark)
    {
        if (IsExistItemByValue(value, itemId))
        {
            throw new UserFriendlyException($"字典类型{Category}字典项值{value}已存在!");
        }

        if (IsExistItemByLabel(label, itemId))
        {
            throw new UserFriendlyException($"字典类型{Category}字典项标签{label}已存在!");
        }

        DictionaryItem? item = DictionaryItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            throw new UserFriendlyException($"字典类型{Category}字典项不存在!");
        }

        item.Modify(value, label, sort, remark);
    }

    /// <summary>
    /// 删除字典项
    /// </summary>
    /// <param name="itemId">字典项Id</param>
    public void DeleteItem(long itemId)
    {
        DictionaryItem? item = DictionaryItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
    }

    private bool IsExistItemByValue(string value, long? excludeItemId)
    {
        return DictionaryItems.Any(x => x.Id != excludeItemId && x.Value == value && !x.IsDeleted);
    }

    private bool IsExistItemByLabel(string label, long? excludeItemId)
    {
        return DictionaryItems.Any(x => x.Id != excludeItemId && x.Label == label && !x.IsDeleted);
    }
}
