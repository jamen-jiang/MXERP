using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SpuCompanies;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 产品公司关联仓储实现
/// </summary>
[ScopedService]
public class SpuCompanyRepository : EfCoreRepository<MXERPDbContext, SpuCompany>, ISpuCompanyRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SpuCompanyRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<SpuCompany>> GetBySpuIdAsync(long spuId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.SpuId == spuId).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<SpuCompany>> GetByCompanyIdAsync(long companyId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
    }
}
