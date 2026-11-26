namespace MXERP.Domain.Processes;

/// <summary>
/// 工序领域服务接口
/// </summary>
public interface IProcessDomainService
{
    /// <summary>
    /// 修改工序(按Id查询后修改)
    /// </summary>
    /// <param name="id">工序Id</param>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="name">工序名称</param>
    /// <param name="price">价格</param>
    /// <param name="cost">成本</param>
    /// <param name="isCFUsable">车房可用</param>
    /// <param name="isKCUsable">库存可用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序聚合</returns>
    Task<Process> ModifyAsync(long id, string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改工序(使用已有聚合)
    /// </summary>
    /// <param name="process">工序聚合</param>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="name">工序名称</param>
    /// <param name="price">价格</param>
    /// <param name="cost">成本</param>
    /// <param name="isCFUsable">车房可用</param>
    /// <param name="isKCUsable">库存可用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Process process, string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成: 工序存在则修改, 不存在则创建
    /// </summary>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="name">工序名称</param>
    /// <param name="price">价格</param>
    /// <param name="cost">成本</param>
    /// <param name="isCFUsable">车房可用</param>
    /// <param name="isKCUsable">库存可用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序聚合</returns>
    Task<Process> ImportGenerateAsync(string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default);
}
