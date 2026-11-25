namespace MXERP.Domain.Jobs;

public interface IJobFactory
{
    /// <summary>
    /// 创建任务
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="beanName">Bean 名称</param>
    /// <param name="methodName">方法名</param>
    /// <param name="cron">Cron 表达式</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回任务</returns>
    Task<Job> CreateAsync(string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default);
}
