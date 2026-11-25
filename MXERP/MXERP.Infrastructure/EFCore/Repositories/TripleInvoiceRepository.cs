using Microsoft.EntityFrameworkCore;
using MXERP.Domain.TripleInvoices;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 三联发票仓储
/// </summary>
[ScopedService]
public class TripleInvoiceRepository : EfCoreRepository<MXERPDbContext, TripleInvoice>, ITripleInvoiceRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public TripleInvoiceRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<TripleInvoice?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TripleInvoice?> GetMaxByStoreInvoiceTitleAsync(string storeInvoiceTitle, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.StoreInvoiceTitle == storeInvoiceTitle).OrderByDescending(x => x.Number).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRetailOrderIdAsync(long retailOrderId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RetailOrderId == retailOrderId, cancellationToken);
    }
}
