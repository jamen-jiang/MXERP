namespace MXERP.Domain.Ranks;

/// <summary>
/// 职级领域服务接口(对应 Java RankDomainService)
/// </summary>
public interface IRankDomainService
{
    /// <summary>
    /// 创建职级
    /// </summary>
    /// <param name="parentId">上级Id</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回职级</returns>
    Task<RankGenerateResult> CreateAsync(long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(职级)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="parentId">上级Id</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回职级</returns>
    Task<RankGenerateResult> ModifyAsync(long id, long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(职级)
    /// </summary>
    /// <param name="rank">rank</param>
    /// <param name="parentId">上级Id</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Rank rank, long? parentId, string name, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// RemoveWithChildrenAsync(职级)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>职级列表</returns>
    Task<List<Rank>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default);
}
