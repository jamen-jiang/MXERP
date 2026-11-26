using Snow.Domain.Repositories;

namespace MXERP.Domain.Attachments;

/// <summary>
/// 附件仓储接口
/// </summary>
public interface IAttachmentRepository : IRepository<Attachment>
{
    /// <summary>
    /// 按持有者类型 + 持有者Id查询列表
    /// </summary>
    /// <param name="ownerType">持有者类型</param>
    /// <param name="ownerId">持有者Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>附件列表</returns>
    Task<List<Attachment>> GetByOwnerTypeAndOwnerIdAsync(string ownerType, long ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按持有者类型 + 持有者Id查询列表(排除指定Id)
    /// <para>ids 为空时查全部, 否则排除 ids</para>
    /// </summary>
    /// <param name="ownerType">持有者类型</param>
    /// <param name="ownerId">持有者Id</param>
    /// <param name="ids">要排除的附件Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>附件列表</returns>
    Task<List<Attachment>> GetByOwnerTypeAndOwnerIdExcludeIdsAsync(string ownerType, long ownerId, IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
