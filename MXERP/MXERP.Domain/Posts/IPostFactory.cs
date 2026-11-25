namespace MXERP.Domain.Posts;

/// <summary>
/// 岗位工厂接口
/// </summary>
public interface IPostFactory
{
    /// <summary>
    /// 创建岗位
    /// </summary>
    /// <param name="name">岗位名称</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="rankId">职级Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="roleIds">角色Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回岗位</returns>
    Task<Post> CreateAsync(string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default);
}
