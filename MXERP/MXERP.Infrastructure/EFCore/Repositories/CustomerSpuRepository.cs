using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.CustomersSpus;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 客人镜种仓储实现
/// </summary>
[ScopedService]
public class CustomerSpuRepository : EfCoreRepository<MXERPDbContext, CustomerSpu>, ICustomerSpuRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public CustomerSpuRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<CustomerSpu?> GetByProductTypeAndCodeAsync(string productType, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ProductType == productType && x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByProductTypeAndCodeAsync(string productType, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.ProductType == productType && x.Code == code, cancellationToken);
    }
}
