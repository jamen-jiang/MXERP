using Snow.Domain.Repositories;

namespace MXERP.Domain.RetailOrderWorkSteps;

/// <summary>
/// 散镜订单工作步骤仓储
/// </summary>
public interface IRetailOrderWorkStepRepository : IRepository<RetailOrderWorkStep>
{
    /// <summary>
    /// 按代码查询
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤，不存在返回 null</returns>
    Task<RetailOrderWorkStep?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤列表</returns>
    Task<List<RetailOrderWorkStep>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按序列查询
    /// </summary>
    /// <param name="sequence">序列</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤，不存在返回 null</returns>
    Task<RetailOrderWorkStep?> GetBySequenceAsync(int sequence, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询除指定Id外的所有散镜订单工作步骤（按序列升序）
    /// </summary>
    /// <param name="excludeId">排除的Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤列表</returns>
    Task<List<RetailOrderWorkStep>> GetListExcludeIdAsync(long excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询除指定Id列表外的所有散镜订单工作步骤（按序列升序）
    /// </summary>
    /// <param name="excludeIds">排除的Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤列表</returns>
    Task<List<RetailOrderWorkStep>> GetListExcludeIdsAsync(IEnumerable<long> excludeIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询所有散镜订单工作步骤（按序列升序）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单工作步骤列表</returns>
    Task<List<RetailOrderWorkStep>> GetAllOrderBySequenceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断代码是否存在
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="id">排除的Id，为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断名称是否存在
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="id">排除的Id，为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);
}
