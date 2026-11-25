using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Ranks;

/// <summary>
/// 职级领域服务(对应 Java RankDomainService): 负责层级编排
/// </summary>
[ScopedService]
public class RankDomainService : IRankDomainService
{
    private readonly IRankRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public RankDomainService(IRankRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<RankGenerateResult> CreateAsync(long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistAsync(name, parentId, null, cancellationToken))
        {
            throw new UserFriendlyException($"同级职级名称{name}已存在!");
        }

        string? level = await GetLevelAsync(parentId, cancellationToken);
        var rank = new Rank(level, parentId, name, sort, remark);
        var ranks = new List<Rank>();
        await SetChildrenLevelAsync(rank, ranks, cancellationToken);
        return new RankGenerateResult(rank, ranks);
    }

    /// <inheritdoc />
    public async Task<RankGenerateResult> ModifyAsync(long id, long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        Rank? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("职级不存在!");
        }

        await ModifyAsync(model, parentId, name, sort, remark, cancellationToken);
        var ranks = new List<Rank>();
        await SetChildrenLevelAsync(model, ranks, cancellationToken);
        return new RankGenerateResult(model, ranks);
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Rank rank, long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistAsync(name, parentId, rank.Id, cancellationToken))
        {
            throw new UserFriendlyException($"同级职级名称{name}已存在!");
        }

        string? level = await GetLevelAsync(parentId, cancellationToken);
        rank.Modify(level, parentId, name, sort, remark);
    }

    /// <inheritdoc />
    public async Task<List<Rank>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        List<Rank> list = await _repository.GetWithChildrenAsync(id, cancellationToken);
        foreach (Rank item in list)
        {
            item.Delete();
        }

        return list;
    }

    /// <summary>
    /// GetLevelAsync(职级)
    /// </summary>
    /// <param name="parentId">上级Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>职级, 不存在返回 null</returns>
    private async Task<string?> GetLevelAsync(long? parentId, CancellationToken cancellationToken)
    {
        if (parentId is null)
        {
            return null;
        }

        Rank? parent = await _repository.FindAsync(parentId.Value, true, cancellationToken);
        if (parent is null)
        {
            throw new UserFriendlyException($"父职级不存在: {parentId}");
        }

        if (parent.Level.IsNullOrWhiteSpace())
        {
            return parent.Id.ToString();
        }

        return parent.Level + "," + parent.Id;
    }

    /// <summary>
    /// SetChildrenLevelAsync(职级)
    /// </summary>
    /// <param name="rank">rank</param>
    /// <param name="childrenList">childrenList</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task SetChildrenLevelAsync(Rank rank, List<Rank> childrenList, CancellationToken cancellationToken)
    {
        if (rank.Id == 0)
        {
            return;
        }

        List<Rank> children = await _repository.GetByParentIdAsync(rank.Id, cancellationToken);
        if (children.Count == 0)
        {
            return;
        }

        foreach (Rank child in children)
        {
            if (rank.Level.IsNullOrWhiteSpace())
            {
                child.ChangeLevel(rank.Id.ToString());
            }
            else
            {
                child.ChangeLevel(rank.Level + "," + rank.Id);
            }

            await SetChildrenLevelAsync(child, childrenList, cancellationToken);
            childrenList.Add(child);
        }
    }
}
