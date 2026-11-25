using Microsoft.EntityFrameworkCore;
using MXERP.Domain.CustomerInvoiceTitleNumbers;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 客户票号仓储实现
/// </summary>
[ScopedService]
public class CustomerInvoiceTitleNumberRepository : EfCoreRepository<MXERPDbContext, CustomerInvoiceTitleNumber>, ICustomerInvoiceTitleNumberRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public CustomerInvoiceTitleNumberRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<CustomerInvoiceTitleNumber?> GetByInvoiceTitleAsync(string invoiceTitle, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.InvoiceTitle == invoiceTitle, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByInvoiceTitleExcludeIdAsync(string invoiceTitle, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.InvoiceTitle == invoiceTitle && x.Id != id, cancellationToken);
    }
}
