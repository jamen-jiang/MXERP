using Snow.Domain.Repositories;

namespace MXERP.Domain.Departments;

/// <summary>
/// 部门仓储(对应 Java DepartmentRepository)
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// 按代码获取部门, 不存在返回 null
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门, 不存在返回 null</returns>
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门及其所有子级
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门列表</returns>
    Task<List<Department>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定上级下的部门列表
    /// </summary>
    /// <param name="parentId">上级Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门列表</returns>
    Task<List<Department>> GetListByParentIdAsync(long parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同一公司下代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeAndCompanyIdExcludeIdAsync(string code, long companyId, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同一公司下名称是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNameAndCompanyIdExcludeIdAsync(string name, long companyId, long? id = null, CancellationToken cancellationToken = default);
}
