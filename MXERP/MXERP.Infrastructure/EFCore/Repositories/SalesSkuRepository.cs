using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SalesSkus;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 销售SKU仓储实现
/// </summary>
[ScopedService]
public class SalesSkuRepository : EfCoreRepository<MXERPDbContext, SalesSku>, ISalesSkuRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SalesSkuRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<SalesSku?> GetByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.AttributesSignature == attributesSignature, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<SalesSku>> GetBySalesSpuIdAsync(long salesSpuId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.SalesSpuId == salesSpuId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.AttributesSignature == attributesSignature, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCompanyIdAndOpcExcludeIdAsync(long companyId, string opc, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.CompanyId == companyId && x.Opc == opc && x.Id != id, cancellationToken);
    }
}
