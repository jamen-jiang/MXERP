namespace MXERP.Domain.Units;

public interface IUnitFactory
{
    /// <summary>
    /// 创建单位
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回单位</returns>
    Task<Unit> CreateAsync(string code, string name, int? sort, string? remark, CancellationToken cancellationToken = default);
}
