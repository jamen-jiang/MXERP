using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码配置仓储接口
/// </summary>
public interface IEndProductCodeConfigRepository : IRepository<EndProductCodeConfig>
{
    /// <summary>
    /// 按产品Id和度数类型查询
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置, 不存在返回 null</returns>
    Task<EndProductCodeConfig?> GetBySpuIdAndDegreesTypeAsync(long spuId, string degreesType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断产品Id和度数类型是否存在(排除指定主键)
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistBySpuIdAndDegreesTypeExcludeIdAsync(long spuId, string degreesType, long? id = null, CancellationToken cancellationToken = default);
}
