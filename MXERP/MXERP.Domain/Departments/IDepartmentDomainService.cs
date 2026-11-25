namespace MXERP.Domain.Departments;

/// <summary>
/// 部门领域服务接口(对应 Java DepartmentDomainService)
/// </summary>
public interface IDepartmentDomainService
{
    /// <summary>
    /// 创建部门
    /// </summary>
    /// <returns>返回部门</returns>
    Task<DepartmentGenerateResult> CreateAsync(
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(部门)
    /// </summary>
    /// <returns>返回部门</returns>
    Task<DepartmentGenerateResult> ModifyAsync(
        long id,
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// RemoveWithChildrenAsync(部门)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门列表</returns>
    Task<List<Department>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default);
}
