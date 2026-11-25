using Snow.Domain.Repositories;

namespace MXERP.Domain.WorkSteps;

/// <summary>
/// 工作步骤仓储(对应 Java WorkStepRepository)
/// </summary>
public interface IWorkStepRepository : IRepository<WorkStep>
{
    /// <summary>
    /// 按代码获取工序, 不存在返回 null
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序, 不存在返回 null</returns>
    Task<WorkStep?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按序号获取工序, 不存在返回 null
    /// </summary>
    /// <param name="sequence">序号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序, 不存在返回 null</returns>
    Task<WorkStep?> GetBySequenceAsync(int sequence, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按主键集合批量获取工序列表
    /// </summary>
    /// <param name="ids">主键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    Task<List<WorkStep>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取排除指定主键后的工序列表
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    Task<List<WorkStep>> GetListExcludeIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取排除指定主键集合后的工序列表
    /// </summary>
    /// <param name="ids">主键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    Task<List<WorkStep>> GetListExcludeIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取全部工序, 按序号升序
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    Task<List<WorkStep>> GetAllOrderBySequenceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 名称是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);
}
