using Snow.Domain.Repositories;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作仓储接口
/// </summary>
public interface IWorkRepository : IRepository<Work>
{
    /// <summary>
    /// 按工作流Id查询工作
    /// </summary>
    /// <param name="workflowId">工作流Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作, 不存在返回 null</returns>
    Task<Work?> GetByWorkflowIdAsync(string workflowId, CancellationToken cancellationToken = default);
}
