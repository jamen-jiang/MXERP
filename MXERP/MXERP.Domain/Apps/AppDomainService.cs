using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Apps;

[ScopedService]
public class AppDomainService : IAppDomainService
{
    private readonly IAppRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">应用仓储</param>
    public AppDomainService(IAppRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<AppGenerateResult> CreateAsync(AppTypeEnum type, long? parentId, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort, CancellationToken cancellationToken = default)
    {
        App app = new App(type, parentId, await GetLevelAsync(parentId, cancellationToken), name, path, component, redirect, license, remark, title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sort);
        List<App> apps = new List<App>();
        await SetChildrenLevelAsync(app, apps, cancellationToken);
        return new AppGenerateResult(app, apps);
    }

    /// <inheritdoc />
    public async Task<AppGenerateResult> ModifyAsync(long id, long? parentId, string? name, string? path, string? component, string? redirect, string? license, string? remark, string title, string? i18nKey, string? icon, bool isKeepAlive, int? fixedIndexInTab, bool isMultiTab, bool isHide, int? sort, CancellationToken cancellationToken = default)
    {
        App? app = await _repository.FindAsync(id, true, cancellationToken);
        if (app is null)
        {
            throw new UserFriendlyException("应用不存在!");
        }

        app.Modify(parentId, await GetLevelAsync(parentId, cancellationToken), name, path, component, redirect, license, remark, title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sort);
        List<App> apps = new List<App>();
        await SetChildrenLevelAsync(app, apps, cancellationToken);
        return new AppGenerateResult(app, apps);
    }

    /// <inheritdoc />
    public async Task<List<App>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        List<App> list = await _repository.GetWithChildrenAsync(id, cancellationToken);
        foreach (App item in list)
        {
            item.Delete();
        }

        return list;
    }

    /// <summary>
    /// 获取层级字符串(无父节点返回 null)
    /// </summary>
    /// <param name="parentId">父Id(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>层级字符串(如 "1,3,5"), 无父节点返回 null</returns>
    private async Task<string?> GetLevelAsync(long? parentId, CancellationToken cancellationToken)
    {
        if (parentId is null)
        {
            return null;
        }

        App? parent = await _repository.FindAsync(parentId.Value, true, cancellationToken);
        if (parent is null)
        {
            throw new UserFriendlyException($"父应用不存在: {parentId}");
        }

        if (string.IsNullOrWhiteSpace(parent.Level))
        {
            return parent.Id.ToString();
        }

        return parent.Level + "," + parent.Id;
    }

    /// <summary>
    /// 递归设置子应用层级
    /// </summary>
    /// <param name="app">应用</param>
    /// <param name="list">子应用收集列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task SetChildrenLevelAsync(App app, List<App> list, CancellationToken cancellationToken)
    {
        if (app.Id == 0)
        {
            return;
        }

        List<App> children = await _repository.GetByParentIdAsync(app.Id, cancellationToken);
        if (children.Count == 0)
        {
            return;
        }

        foreach (App child in children)
        {
            if (string.IsNullOrWhiteSpace(app.Level))
            {
                child.ChangeLevel(app.Id.ToString());
            }
            else
            {
                child.ChangeLevel(app.Level + "," + app.Id);
            }

            await SetChildrenLevelAsync(child, list, cancellationToken);
            list.Add(child);
        }
    }
}
