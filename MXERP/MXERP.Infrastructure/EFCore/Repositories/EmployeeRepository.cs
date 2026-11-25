using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Employees;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 员工仓储实现(对应 Java 的 EmployeeRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class EmployeeRepository : EfCoreRepository<MXERPDbContext, Employee>, IEmployeeRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public EmployeeRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByBarCodeExcludeIdAsync(string barCode, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.BarCode == barCode && x.Id != id, cancellationToken);
    }
}
