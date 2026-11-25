using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Jobs;

/// <summary>
/// 定时任务(对应 Java Job)
/// </summary>
public class Job : AuditedAggregateRoot
{
    /// <summary>
    /// 默认 cron 表达式: 每天中午 12 点
    /// </summary>
    private const string DefaultCron = "0 0 12 * * ?";

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// Bean 名称
    /// </summary>
    public string BeanName { get; private set; } = string.Empty;
    /// <summary>
    /// 方法名
    /// </summary>
    public string MethodName { get; private set; } = string.Empty;
    /// <summary>
    /// Cron 表达式
    /// </summary>
    public string Cron { get; private set; } = DefaultCron;
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Job()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="beanName">Bean 名称</param>
    /// <param name="methodName">方法名</param>
    /// <param name="cron">Cron 表达式</param>
    /// <param name="remark">备注</param>
    internal Job(string name, string beanName, string methodName, string? cron, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("任务名称不能为空");
        }

        if (beanName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Bean名称不能为空");
        }

        if (methodName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("方法名称不能为空");
        }

        Name = name.Trim();
        BeanName = beanName.Trim();
        MethodName = methodName.Trim();
        if (cron.IsNullOrWhiteSpace())
        {
            Cron = DefaultCron;
        }
        else
        {
            Cron = cron.Trim();
        }

        Remark = remark;
        IsEnable = false;
    }

    /// <summary>
    /// 修改任务
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="beanName">Bean 名称</param>
    /// <param name="methodName">方法名</param>
    /// <param name="cron">Cron 表达式</param>
    /// <param name="remark">备注</param>
    internal void Modify(string name, string beanName, string methodName, string? cron, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("任务名称不能为空");
        }

        if (beanName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Bean名称不能为空");
        }

        if (methodName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("方法名称不能为空");
        }

        Name = name.Trim();
        BeanName = beanName.Trim();
        MethodName = methodName.Trim();
        if (cron.IsNullOrWhiteSpace())
        {
            Cron = DefaultCron;
        }
        else
        {
            Cron = cron.Trim();
        }

        Remark = remark;
    }

    /// <summary>
    /// 启用任务
    /// </summary>
    public void Start()
    {
        IsEnable = true;
    }

    /// <summary>
    /// 停用任务
    /// </summary>
    public void Stop()
    {
        IsEnable = false;
    }

    /// <summary>
    /// 删除任务: 软删除 + 停用
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        Stop();
    }
}
