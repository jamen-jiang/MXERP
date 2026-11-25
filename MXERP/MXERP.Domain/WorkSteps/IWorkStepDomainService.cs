namespace MXERP.Domain.WorkSteps;

/// <summary>
/// 工作步骤领域服务接口(对应 Java WorkStepDomainService)
/// </summary>
public interface IWorkStepDomainService
{
    /// <summary>
    /// 创建工序
    /// </summary>
    /// <returns>返回工序</returns>
    Task<WorkStepGenerateResult> CreateAsync(
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        int prevSequence,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(工序)
    /// </summary>
    /// <returns>返回工序</returns>
    Task<WorkStepGenerateResult> ModifyAsync(
        long id,
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        int prevSequence,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(工序)
    /// </summary>
    Task ModifyAsync(
        WorkStep model,
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// RemoveAndResequenceAsync(工序)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    Task<List<WorkStep>> RemoveAndResequenceAsync(long id, CancellationToken cancellationToken = default);
}
