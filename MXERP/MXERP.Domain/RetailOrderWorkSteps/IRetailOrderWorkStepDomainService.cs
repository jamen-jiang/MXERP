namespace MXERP.Domain.RetailOrderWorkSteps;

/// <summary>
/// 散镜订单工作步骤领域服务，负责跨聚合的序列编排
/// </summary>
public interface IRetailOrderWorkStepDomainService
{
    /// <summary>
    /// 创建（含序列编排）
    /// </summary>
    /// <param name="departmentId">组织机构Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="prevSequence">上一个序列（0 表示插入到最前面）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>生成结果</returns>
    Task<RetailOrderWorkStepGenerateResult> CreateAsync(long departmentId, string code, string name, string? remark, int prevSequence, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改（按Id查询后修改，含序列编排）
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="departmentId">组织机构Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="prevSequence">上一个序列（0 表示插入到最前面）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>生成结果</returns>
    Task<RetailOrderWorkStepGenerateResult> ModifyAsync(long id, long departmentId, string code, string name, string? remark, int prevSequence, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改（使用已有聚合，避免重复查询）
    /// </summary>
    /// <param name="model">散镜订单工作步骤聚合</param>
    /// <param name="departmentId">组织机构Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task ModifyAsync(RetailOrderWorkStep model, long departmentId, string code, string name, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除指定步骤并重新编号
    /// </summary>
    /// <param name="id">待删除的Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>需要持久化的聚合列表（含已标记删除的 + 重新编号的）</returns>
    Task<List<RetailOrderWorkStep>> RemoveAndResequenceAsync(long? id, CancellationToken cancellationToken = default);
}
