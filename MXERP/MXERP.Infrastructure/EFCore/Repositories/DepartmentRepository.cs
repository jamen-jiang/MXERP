using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Departments;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 部门仓储实现(对应 Java DepartmentRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class DepartmentRepository : EfCoreRepository<MXERPDbContext, Department>, IDepartmentRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public DepartmentRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Department>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        string idStr = id.ToString();
        return await (await GetQueryableAsync())
            .Where(x => x.Id == id
                || (x.Level != null && (x.Level == idStr || x.Level.StartsWith(idStr + ",") || x.Level.EndsWith("," + idStr) || x.Level.Contains("," + idStr + ","))))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Department>> GetListByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.ParentId == parentId).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeAndCompanyIdExcludeIdAsync(string code, long companyId, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code && x.CompanyId == companyId && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameAndCompanyIdExcludeIdAsync(string name, long companyId, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.CompanyId == companyId && x.Id != id, cancellationToken);
    }
}
