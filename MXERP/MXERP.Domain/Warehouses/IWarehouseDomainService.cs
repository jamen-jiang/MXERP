namespace MXERP.Domain.Warehouses;

public interface IWarehouseDomainService
{
    /// <summary>
    /// MarkDefaultAsync(仓库)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回仓库</returns>
    Task<MarkDefaultResult> MarkDefaultAsync(long id, CancellationToken cancellationToken = default);
}
