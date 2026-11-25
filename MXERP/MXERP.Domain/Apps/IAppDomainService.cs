namespace MXERP.Domain.Apps;

/// <summary>
/// 应用领域服务接口(负责创建、修改和层级编排)
/// </summary>
public interface IAppDomainService
{
    /// <summary>
    /// 创建应用(含层级编排)
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="parentId">父Id(可空)</param>
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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>生成结果(应用 + 子应用列表)</returns>
    Task<AppGenerateResult> CreateAsync(AppTypeEnum type, long? parentId, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改应用(含层级编排)
    /// </summary>
    /// <param name="id">应用Id</param>
    /// <param name="parentId">父Id(可空)</param>
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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>生成结果(应用 + 子应用列表)</returns>
    Task<AppGenerateResult> ModifyAsync(long id, long? parentId, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除自身及所有子孙应用
    /// </summary>
    /// <param name="id">应用Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>已标记删除的聚合列表</returns>
    Task<List<App>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default);
}
