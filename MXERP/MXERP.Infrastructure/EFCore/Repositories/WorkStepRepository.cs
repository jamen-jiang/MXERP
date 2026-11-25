using Microsoft.EntityFrameworkCore;
using MXERP.Domain.WorkSteps;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 工序仓储实现(对应 Java WorkStepRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class WorkStepRepository : EfCoreRepository<MXERPDbContext, WorkStep>, IWorkStepRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public WorkStepRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<WorkStep?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkStep?> GetBySequenceAsync(int sequence, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Sequence == sequence, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<WorkStep>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<WorkStep>> GetListExcludeIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Id != id).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<WorkStep>> GetListExcludeIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => !ids.Contains(x.Id)).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<WorkStep>> GetAllOrderBySequenceAsync(CancellationToken cancellationToken = default)
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
