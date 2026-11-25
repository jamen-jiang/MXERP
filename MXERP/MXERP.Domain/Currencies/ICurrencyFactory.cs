namespace MXERP.Domain.Currencies;

/// <summary>
/// 币种工厂(对应 Java CurrencyFactory)
/// </summary>
public interface ICurrencyFactory
{
    /// <summary>
    /// 创建货币
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="symbol">符号</param>
    /// <param name="acceptRate">接受汇率</param>
    /// <param name="adjustRate">调整汇率</param>
    /// <param name="isStandard">是否标准</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回货币</returns>
    Task<Currency> CreateAsync(string code, string name, string symbol, decimal acceptRate, decimal adjustRate, bool isStandard, bool isEnable, CancellationToken cancellationToken = default);
}
