namespace MXERP.Domain.Posts;

/// <summary>
/// 岗位领域服务接口
/// </summary>
public interface IPostDomainService
{
    /// <summary>
    /// 修改岗位(按Id查询后修改)
    /// </summary>
    /// <param name="id">岗位Id</param>
    /// <param name="name">岗位名称</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="rankId">职级Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="roleIds">角色Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回岗位</returns>
    Task<Post> ModifyAsync(long id, string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改岗位(使用已有聚合)
    /// </summary>
    /// <param name="post">岗位聚合</param>
    /// <param name="name">岗位名称</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="rankId">职级Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="roleIds">角色Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Post post, string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default);
}
