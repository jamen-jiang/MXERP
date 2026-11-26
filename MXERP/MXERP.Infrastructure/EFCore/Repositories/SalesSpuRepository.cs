using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 销售产品仓储实现
/// </summary>
[ScopedService]
public class SalesSpuRepository : EfCoreRepository<MXERPDbContext, SalesSpu>, ISalesSpuRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SalesSpuRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<SalesSpu?> GetByProductTypeAndCompanyIdAndCodeAsync(string productType, long companyId, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.ProductType == productType && x.CompanyId == companyId && x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByProductTypeAndCompanyIdAndCodeExcludeIdAsync(string productType, long companyId, string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.ProductType == productType && x.CompanyId == companyId && x.Code == code && x.Id != id, cancellationToken);
    }
}
