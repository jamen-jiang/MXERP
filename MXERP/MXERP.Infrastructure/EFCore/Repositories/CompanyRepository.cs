using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Companies;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 公司仓储实现(对应 Java 的 CompanyRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class CompanyRepository : EfCoreRepository<MXERPDbContext, Company>, ICompanyRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public CompanyRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Company?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);
    }
}
