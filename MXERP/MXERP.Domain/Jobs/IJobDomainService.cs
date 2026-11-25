namespace MXERP.Domain.Jobs;

public interface IJobDomainService
{
    /// <summary>
    /// ModifyAsync(任务)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="name">名称</param>
    /// <param name="beanName">Bean 名称</param>
    /// <param name="methodName">方法名</param>
    /// <param name="cron">Cron 表达式</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回任务</returns>
    Task<Job> ModifyAsync(long id, string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(任务)
    /// </summary>
    /// <param name="job">job</param>
    /// <param name="name">名称</param>
    /// <param name="beanName">Bean 名称</param>
    /// <param name="methodName">方法名</param>
    /// <param name="cron">Cron 表达式</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Job job, string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default);
}
