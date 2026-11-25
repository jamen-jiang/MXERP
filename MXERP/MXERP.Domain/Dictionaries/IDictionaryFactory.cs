namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典工厂
/// </summary>
public interface IDictionaryFactory
{
    /// <summary>
    /// 创建字典
    /// </summary>
    /// <param name="category">字典类型</param>
    /// <param name="name">字典类型名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>字典聚合</returns>
    Task<Dictionary> CreateAsync(string category, string name, string? remark, CancellationToken cancellationToken = default);
}
