namespace MXERP.Domain.TaxRates;

public interface ITaxRateFactory
{
    /// <summary>
    /// 创建税率
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="rate">rate</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回税率</returns>
    Task<TaxRate> CreateAsync(string code, string name, decimal rate, int? sort, string? remark, CancellationToken cancellationToken = default);
}
