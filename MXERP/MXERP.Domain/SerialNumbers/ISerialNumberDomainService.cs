namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号领域服务接口
/// </summary>
public interface ISerialNumberDomainService
{
    /// <summary>
    /// 修改流水号(按Id查询后修改)
    /// </summary>
    /// <param name="id">流水号Id</param>
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
    Task<SerialNumber> ModifyAsync(long id, string type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, string? rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改流水号(使用已有聚合, 避免重复查询)
    /// </summary>
    /// <param name="serialNumber">流水号聚合</param>
    /// <param name="type">类型</param>
    /// <param name="prefix">前缀</param>
    /// <param name="dateFormat">时间格式(可空)</param>
    /// <param name="pad">补充数量</param>
    /// <param name="padSymbol">补充符号</param>
    /// <param name="isSplit">是否分割</param>
    /// <param name="splitSymbol">分割符号</param>
    /// <param name="rule">规则(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(SerialNumber serialNumber, string type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, string? rule, CancellationToken cancellationToken = default);
}
