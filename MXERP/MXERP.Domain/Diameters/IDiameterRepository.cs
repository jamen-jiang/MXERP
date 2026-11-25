using Snow.Domain.Repositories;

namespace MXERP.Domain.Diameters;

public interface IDiameterRepository : IRepository<Diameter>
{
    /// <summary>
    /// 按尺寸获取直径, 不存在返回 null
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>直径, 不存在返回 null</returns>
    Task<Diameter?> GetBySizeAsync(int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取默认直径, 不存在返回 null
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>直径, 不存在返回 null</returns>
    Task<Diameter?> GetDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 尺寸是否已存在(排除指定主键)
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistBySizeExcludeIdAsync(int size, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 颜色是否已存在(排除指定主键)
    /// </summary>
    /// <param name="color">颜色</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByColorExcludeIdAsync(string color, long? id = null, CancellationToken cancellationToken = default);
}
