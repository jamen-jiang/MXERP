namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典领域服务
/// </summary>
public interface IDictionaryDomainService
{
    /// <summary>
    /// 修改(按Id查询后修改)
    /// </summary>
    /// <param name="id">字典Id</param>
    /// <param name="category">字典类型</param>
    /// <param name="name">字典类型名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>字典聚合</returns>
    Task<Dictionary> ModifyAsync(long id, string category, string name, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步字典项列表(输入里有则更新/新增, 没有则标记删除)
    /// </summary>
    /// <param name="dictionary">字典</param>
    /// <param name="dictionaryItems">字典项列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncDictionaryItemsAsync(Dictionary dictionary, IReadOnlyList<DictionaryItemData>? dictionaryItems, CancellationToken cancellationToken = default);
}
