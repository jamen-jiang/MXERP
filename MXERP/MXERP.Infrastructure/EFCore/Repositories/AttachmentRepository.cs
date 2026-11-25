using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Attachments;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 附件仓储实现(对应 Java 的 AttachmentRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class AttachmentRepository : EfCoreRepository<MXERPDbContext, Attachment>, IAttachmentRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public AttachmentRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<Attachment>> GetByOwnerTypeAndOwnerIdAsync(string ownerType, long ownerId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Attachment>> GetByOwnerTypeAndOwnerIdExcludeIdsAsync(string ownerType, long ownerId, IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        IQueryable<Attachment> query = (await GetQueryableAsync())
            .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId);
        if (ids.Count > 0)
        {
            query = query.Where(x => !ids.Contains(x.Id));
        }

        return await query.ToListAsync(cancellationToken);
    }
}
