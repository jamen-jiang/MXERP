namespace MXERP.Domain.SpuBrands;

/// <summary>
/// 产品品牌工厂接口
/// </summary>
public interface ISpuBrandFactory
{
    /// <summary>
    /// 创建品牌
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>品牌聚合</returns>
    Task<SpuBrand> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default);
}
