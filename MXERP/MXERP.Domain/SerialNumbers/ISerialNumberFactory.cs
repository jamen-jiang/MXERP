using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号工厂接口
/// </summary>
public interface ISerialNumberFactory
{
    /// <summary>
    /// 创建流水号
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="prefix">前缀</param>
    /// <param name="dateFormat">时间格式(可空)</param>
    /// <param name="pad">补充数量</param>
    /// <param name="padSymbol">补充符号</param>
    /// <param name="isSplit">是否分割</param>
    /// <param name="splitSymbol">分割符号</param>
    /// <param name="rule">规则(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>流水号聚合</returns>
    Task<SerialNumber> CreateAsync(string type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, string? rule, CancellationToken cancellationToken = default);
}
