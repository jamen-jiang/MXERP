namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号生成器接口
/// </summary>
public interface ISerialNumberGenerator
{
    /// <summary>
    /// 生成下一个流水号
    /// </summary>
    /// <param name="type">流水号类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>生成的流水号</returns>
    Task<string> NextAsync(SerialNumberTypeEnum type, CancellationToken cancellationToken = default);
}
