using Microsoft.EntityFrameworkCore;
using MXERP.Domain.ExternalRetailOrderTrackings;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 外部散镜订单跟踪仓储
/// </summary>
[ScopedService]
public class ExternalRetailOrderTrackingRepository : EfCoreRepository<MXERPDbContext, ExternalRetailOrderTracking>, IExternalRetailOrderTrackingRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public ExternalRetailOrderTrackingRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<ExternalRetailOrderTracking?> GetByNumberAndWorkStepIdAsync(string number, long workStepId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ExternalRetailOrderNumber == number && x.WorkStepId == workStepId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ExternalRetailOrderTracking>> GetListByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.ExternalRetailOrderNumber == number).ToListAsync(cancellationToken);
    }
}
