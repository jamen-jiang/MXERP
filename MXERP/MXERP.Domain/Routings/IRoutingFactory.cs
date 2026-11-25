namespace MXERP.Domain.Routings;

/// <summary>
/// 工艺路线工厂接口
/// </summary>
public interface IRoutingFactory
{
    /// <summary>
    /// 创建路线
    /// </summary>
    /// <param name="code">路线代码</param>
    /// <param name="name">路线名称</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路线聚合</returns>
    Task<Routing> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default);
}
