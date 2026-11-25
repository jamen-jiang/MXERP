namespace MXERP.Domain.Diameters;

public interface IDiameterDomainService
{
    /// <summary>
    /// ModifyAsync(直径)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="color">颜色</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回直径</returns>
    Task<Diameter> ModifyAsync(long id, string color, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// MarkDefaultAsync(直径)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回直径</returns>
    Task<MarkDefaultResult> MarkDefaultAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成直径(存在则修改, 不存在则创建)
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="color">颜色</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回直径</returns>
    Task<Diameter> ImportGenerateAsync(int size, string color, string? remark, CancellationToken cancellationToken = default);
}
