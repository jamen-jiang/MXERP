using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码配置领域服务接口
/// </summary>
public interface IEndProductCodeConfigDomainService
{
    /// <summary>
    /// 修改成品代码配置(按Id查询后修改)
    /// </summary>
    /// <param name="id">配置Id</param>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置聚合</returns>
    Task<EndProductCodeConfig> ModifyAsync(long id, string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改成品代码配置(使用已有聚合)
    /// </summary>
    /// <param name="model">配置聚合</param>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(EndProductCodeConfig model, string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成: 配置存在则修改, 不存在则创建
    /// </summary>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置聚合</returns>
    Task<EndProductCodeConfig> ImportGenerateAsync(string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default);
}
