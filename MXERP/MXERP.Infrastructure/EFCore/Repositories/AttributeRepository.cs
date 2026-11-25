using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Attributes;
using AttributeEntity = MXERP.Domain.Attributes.Attribute;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 属性仓储实现
/// </summary>
[ScopedService]
public class AttributeRepository : EfCoreRepository<MXERPDbContext, AttributeEntity>, IAttributeRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public AttributeRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<AttributeEntity?> GetByProductTypeAndScopeAndCodeAsync(string productType, string scope, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.ProductType == productType && x.Scope == scope && x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AttributeEntity>> GetListByProductTypeAndScopeAsync(string productType, string scope, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.ProductType == productType && x.Scope == scope)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByProductTypeAndScopeAndCodeExcludeIdAsync(string productType, string scope, string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.ProductType == productType && x.Scope == scope && x.Code == code && x.Id != id, cancellationToken);
    }
}
