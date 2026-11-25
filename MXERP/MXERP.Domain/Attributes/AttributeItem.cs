using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性项
/// </summary>
public class AttributeItem : DeletionAuditedEntity
{
    /// <summary>
    /// 属性Id
    /// </summary>
    public long? AttributeId { get; private set; }

    /// <summary>
    /// 属性值
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// 属性值名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected AttributeItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="value">属性值</param>
    /// <param name="name">属性值名称</param>
    /// <param name="isDefault">是否默认</param>
    /// <param name="sort">排序</param>
    internal AttributeItem(string value, string name, bool isDefault, int? sort)
    {
        string? trimmedValue = value?.Trim();
        string? trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedValue))
        {
            throw new UserFriendlyException("属性值不能为空");
        }

        if (string.IsNullOrEmpty(trimmedName))
        {
            throw new UserFriendlyException("属性值名称不能为空");
        }

        Value = trimmedValue;
        Name = trimmedName;
        IsDefault = isDefault;
        Sort = sort;
    }

    /// <summary>
    /// 修改属性项
    /// </summary>
    /// <param name="name">属性值名称</param>
    /// <param name="isDefault">是否默认</param>
    /// <param name="sort">排序</param>
    public void Modify(string name, bool isDefault, int? sort)
    {
        string? trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedName))
        {
            throw new UserFriendlyException("属性值名称不能为空");
        }

        Name = trimmedName;
        IsDefault = isDefault;
        Sort = sort;
    }

    /// <summary>
    /// 设置为默认项
    /// </summary>
    public void SetAsDefault()
    {
        IsDefault = true;
    }

    /// <summary>
    /// 取消默认项
    /// </summary>
    public void UnsetDefault()
    {
        IsDefault = false;
    }
}
