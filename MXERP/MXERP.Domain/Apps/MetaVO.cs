using Snow.Domain.ValueObjects;

namespace MXERP.Domain.Apps;

/// <summary>
/// 路由元数据(对应 Java 的 MetaVO, 继承 ValueObject 作为 App 的内嵌值对象)
/// </summary>
public class MetaVO : ValueObject
{
    /// <summary>
    /// 模块标题
    /// </summary>
    public string Title { get; private set; } = string.Empty;
    /// <summary>
    /// 国际化Key(可空)
    /// </summary>
    public string? I18nKey { get; private set; }
    /// <summary>
    /// 图标(可空)
    /// </summary>
    public string? Icon { get; private set; }
    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsKeepAlive { get; private set; }
    /// <summary>
    /// 固定页签序号(可空)
    /// </summary>
    public int? FixedIndexInTab { get; private set; }
    /// <summary>
    /// 是否多页签
    /// </summary>
    public bool IsMultiTab { get; private set; }
    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHide { get; private set; }
    /// <summary>
    /// 排序(可空)
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected MetaVO()
    {
    }

    /// <summary>
    /// 全参构造器
    /// </summary>
    /// <param name="title">模块标题</param>
    /// <param name="i18nKey">国际化Key(可空)</param>
    /// <param name="icon">图标(可空)</param>
    /// <param name="isKeepAlive">是否缓存</param>
    /// <param name="fixedIndexInTab">固定页签序号(可空)</param>
    /// <param name="isMultiTab">是否多页签</param>
    /// <param name="isHide">是否隐藏</param>
    /// <param name="sort">排序(可空)</param>
    internal MetaVO(string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort)
    {
        Title = title?.Trim() ?? string.Empty;
        I18nKey = i18nKey?.Trim();
        Icon = icon?.Trim();
        IsKeepAlive = isKeepAlive;
        FixedIndexInTab = fixedIndexInTab;
        IsMultiTab = isMultiTab;
        IsHide = isHide;
        Sort = sort;
    }

    /// <summary>
    /// 返回参与判等的属性
    /// </summary>
    /// <returns>属性序列</returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return new object?[] { Title, I18nKey, Icon, IsKeepAlive, FixedIndexInTab, IsMultiTab, IsHide, Sort };
    }
}
