using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Apps;

/// <summary>
/// 应用(对应 Java App)
/// </summary>
public class App : AuditedAggregateRoot
{
    /// <summary>
    /// 类型
    /// </summary>
    public AppTypeEnum Type { get; private set; }
    /// <summary>
    /// 父Id(可空)
    /// </summary>
    public long? ParentId { get; private set; }
    /// <summary>
    /// 应用级别字符串(可空)
    /// </summary>
    public string? Level { get; private set; }
    /// <summary>
    /// 名称(可空)
    /// </summary>
    public string? Name { get; private set; }
    /// <summary>
    /// 路径(可空)
    /// </summary>
    public string? Path { get; private set; }
    /// <summary>
    /// 组件路径(可空)
    /// </summary>
    public string? Component { get; private set; }
    /// <summary>
    /// 路由重定向(可空)
    /// </summary>
    public string? Redirect { get; private set; }
    /// <summary>
    /// 许可证(可空)
    /// </summary>
    public string? License { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 路由元数据
    /// </summary>
    public MetaVO Meta { get; private set; } = null!;

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected App()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="parentId">父Id(可空)</param>
    /// <param name="level">层级字符串(可空)</param>
    /// <param name="name">名称(可空)</param>
    /// <param name="path">路径(可空)</param>
    /// <param name="component">组件路径(可空)</param>
    /// <param name="redirect">路由重定向(可空)</param>
    /// <param name="license">许可证(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="title">模块标题</param>
    /// <param name="i18nKey">国际化Key(可空)</param>
    /// <param name="icon">图标(可空)</param>
    /// <param name="isKeepAlive">是否缓存</param>
    /// <param name="fixedIndexInTab">固定页签序号(可空)</param>
    /// <param name="isMultiTab">是否多页签</param>
    /// <param name="isHide">是否隐藏</param>
    /// <param name="sort">排序(可空)</param>
    internal App(AppTypeEnum type, long? parentId, string? level, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort)
    {
        Type = type;
        ParentId = parentId;
        Level = level?.Trim();
        Name = name?.Trim();
        Path = path?.Trim();
        Component = component?.Trim();
        Redirect = redirect?.Trim();
        License = license?.Trim();
        Remark = remark;
        Meta = new MetaVO(title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sort);
    }

    /// <summary>
    /// 修改应用
    /// </summary>
    /// <param name="parentId">父Id(可空)</param>
    /// <param name="level">层级字符串(可空)</param>
    /// <param name="name">名称(可空)</param>
    /// <param name="path">路径(可空)</param>
    /// <param name="component">组件路径(可空)</param>
    /// <param name="redirect">路由重定向(可空)</param>
    /// <param name="license">许可证(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="title">模块标题</param>
    /// <param name="i18nKey">国际化Key(可空)</param>
    /// <param name="icon">图标(可空)</param>
    /// <param name="isKeepAlive">是否缓存</param>
    /// <param name="fixedIndexInTab">固定页签序号(可空)</param>
    /// <param name="isMultiTab">是否多页签</param>
    /// <param name="isHide">是否隐藏</param>
    /// <param name="sort">排序(可空)</param>
    internal void Modify(long? parentId, string? level, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort)
    {
        if (parentId is not null && parentId == Id)
        {
            throw new UserFriendlyException("上级应用不能是自己!");
        }

        ParentId = parentId;
        Level = level;
        Name = name?.Trim();
        Path = path?.Trim();
        Component = component?.Trim();
        Redirect = redirect?.Trim();
        License = license?.Trim();
        Remark = remark;
        Meta = new MetaVO(title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sort);
    }

    /// <summary>
    /// 变更层级字符串
    /// </summary>
    /// <param name="level">层级字符串</param>
    internal void ChangeLevel(string level)
    {
        Level = level;
    }


}
