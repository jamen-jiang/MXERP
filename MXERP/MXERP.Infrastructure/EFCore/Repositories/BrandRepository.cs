using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Brands;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>品牌仓储实现: 复用框架的 EfCoreRepository, 只补按代码查询</summary>
[ScopedService]
public class BrandRepository : EfCoreRepository<MXERPDbContext, Brand>, IBrandRepository
{
    /// <summary>构造器</summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public BrandRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Brand?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code, cancellationToken);
    }
}
