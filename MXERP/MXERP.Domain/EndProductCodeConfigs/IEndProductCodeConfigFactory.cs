namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码配置工厂接口
/// </summary>
public interface IEndProductCodeConfigFactory
{
    /// <summary>
    /// 创建配置
    /// </summary>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置聚合</returns>
    Task<EndProductCodeConfig> CreateAsync(string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default);
}
