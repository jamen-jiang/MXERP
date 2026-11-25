namespace MXERP.Domain.Routings;

/// <summary>
/// 工艺路线领域服务接口
/// </summary>
public interface IRoutingDomainService
{
    /// <summary>
    /// 创建路线(含工序行)
    /// </summary>
    /// <param name="code">路线代码</param>
    /// <param name="name">路线名称</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="steps">工序行列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路线聚合</returns>
    Task<Routing> CreateAsync(string code, string name, string? remark, List<RoutingStepData>? steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改路线(含工序行整表同步)
    /// </summary>
    /// <param name="id">路线Id</param>
    /// <param name="name">路线名称</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="steps">工序行列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路线聚合</returns>
    Task<Routing> ModifyAsync(long id, string name, string? remark, List<RoutingStepData>? steps, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步工序行(按 WorkStepId 匹配: 删除不在输入中的、更新已存在的、添加新的)
    /// </summary>
    /// <param name="routing">路线聚合</param>
    /// <param name="steps">目标工序行列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncRoutingStepsAsync(Routing routing, List<RoutingStepData>? steps, CancellationToken cancellationToken = default);
}
