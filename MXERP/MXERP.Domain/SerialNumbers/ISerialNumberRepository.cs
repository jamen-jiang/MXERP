using Snow.Domain.Repositories;

namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号仓储接口
/// </summary>
public interface ISerialNumberRepository : IRepository<SerialNumber>
{
    /// <summary>
    /// 按类型查询流水号
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>流水号, 不存在返回 null</returns>
    Task<SerialNumber?> GetByTypeAsync(SerialNumberTypeEnum type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断类型是否存在(排除指定主键)
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByTypeExcludeIdAsync(SerialNumberTypeEnum type, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断前缀是否存在(排除指定主键)
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByPrefixExcludeIdAsync(string prefix, long? id = null, CancellationToken cancellationToken = default);
}
