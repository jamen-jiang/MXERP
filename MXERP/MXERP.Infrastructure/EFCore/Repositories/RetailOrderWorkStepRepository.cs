using Microsoft.EntityFrameworkCore;
using MXERP.Domain.RetailOrderWorkSteps;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 散镜订单工作步骤仓储实现
/// </summary>
[ScopedService]
public class RetailOrderWorkStepRepository : EfCoreRepository<MXERPDbContext, RetailOrderWorkStep>, IRetailOrderWorkStepRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public RetailOrderWorkStepRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<RetailOrderWorkStep?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderWorkStep>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        return await (await GetQueryableAsync()).Where(x => idList.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RetailOrderWorkStep?> GetBySequenceAsync(int sequence, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Sequence == sequence, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderWorkStep>> GetListExcludeIdAsync(long excludeId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Id != excludeId).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderWorkStep>> GetListExcludeIdsAsync(IEnumerable<long> excludeIds, CancellationToken cancellationToken = default)
    {
        List<long> idList = excludeIds.ToList();
        IQueryable<RetailOrderWorkStep> queryable = await GetQueryableAsync();
        if (idList.Count == 0)
        {
            return await queryable.OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
        }

        return await queryable.Where(x => !idList.Contains(x.Id)).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderWorkStep>> GetAllOrderBySequenceAsync(CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }
}
