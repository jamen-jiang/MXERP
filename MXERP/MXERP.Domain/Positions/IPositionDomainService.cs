namespace MXERP.Domain.Positions;

public interface IPositionDomainService
{
    /// <summary>
    /// ModifyAsync(职位)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="category">分类</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称</param>
    /// <param name="endProductCode">成品代码</param>
    /// <param name="semiProductCode">半成品代码</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回职位</returns>
    Task<Position> ModifyAsync(long id, string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(职位)
    /// </summary>
    /// <param name="position">position</param>
    /// <param name="category">分类</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称</param>
    /// <param name="endProductCode">成品代码</param>
    /// <param name="semiProductCode">半成品代码</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Position position, string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark, CancellationToken cancellationToken = default);
}
