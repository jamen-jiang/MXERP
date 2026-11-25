using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Skus;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// SKU仓储实现
/// </summary>
[ScopedService]
public class SkuRepository : EfCoreRepository<MXERPDbContext, Sku>, ISkuRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SkuRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<Sku>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Sku?> GetByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.AttributesSignature == attributesSignature, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Sku>> GetListBySpuIdAsync(long spuId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.SpuId == spuId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.AttributesSignature == attributesSignature, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistBySpuIdAndOpcExcludeIdAsync(long spuId, string opc, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.SpuId == spuId && x.Opc == opc && x.Id != id, cancellationToken);
    }
}
