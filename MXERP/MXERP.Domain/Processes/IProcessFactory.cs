namespace MXERP.Domain.Processes;

/// <summary>
/// 工序工厂接口
/// </summary>
public interface IProcessFactory
{
    /// <summary>
    /// 创建工序
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
    Task<Process> CreateAsync(ProcessTypeEnum type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default);
}
