using Snow.Domain.Repositories;

namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典仓储
/// </summary>
public interface IDictionaryRepository : IRepository<Dictionary>
{
    /// <summary>
    /// 判断字典类型是否存在(排除指定主键)
    /// </summary>
    /// <param name="category">字典类型</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCategoryExcludeIdAsync(string category, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断字典类型名称是否存在(排除指定主键)
    /// </summary>
    /// <param name="name">字典类型名称</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);
}
