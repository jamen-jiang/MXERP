using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Currencies;

/// <summary>
/// 币种(对应 Java Currency)
/// </summary>
public class Currency : AuditedAggregateRoot
{
    /// <summary>
    /// 币种代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 币种名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 币种符号
    /// </summary>
    public string Symbol { get; private set; } = string.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    public decimal AcceptRate { get; private set; }

    /// <summary>
    /// 调整率
    /// </summary>
    public decimal AdjustRate { get; private set; }

    /// <summary>
    /// 是否本位币
    /// </summary>
    public bool IsStandard { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; private set; }

    /// <summary>
    /// EF Core 物化用
    /// </summary>
    private Currency()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="symbol">符号</param>
    /// <param name="acceptRate">接受汇率</param>
    /// <param name="adjustRate">调整汇率</param>
    /// <param name="isStandard">是否标准</param>
    /// <param name="isEnable">是否启用</param>
    internal Currency(string code, string name, string symbol, decimal acceptRate, decimal adjustRate, bool isStandard, bool isEnable)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("币种代码不能为空");
        }

        Code = code.Trim();
        Name = (name ?? string.Empty).Trim();
        Symbol = (symbol ?? string.Empty).Trim();
        AcceptRate = acceptRate;
        AdjustRate = adjustRate;
        IsStandard = isStandard;
        IsEnable = isEnable;
    }

    /// <summary>
    /// 修改币种
    /// </summary>
    public void Modify(string name, string symbol, decimal acceptRate, decimal adjustRate, bool isStandard, bool isEnable)
    {
        Name = (name ?? string.Empty).Trim();
        Symbol = (symbol ?? string.Empty).Trim();
        AcceptRate = acceptRate;
        AdjustRate = adjustRate;
        IsStandard = isStandard;
        IsEnable = isEnable;
    }
}
