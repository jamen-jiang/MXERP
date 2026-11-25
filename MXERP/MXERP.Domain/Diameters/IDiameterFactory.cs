namespace MXERP.Domain.Diameters;

public interface IDiameterFactory
{
    /// <summary>
    /// 创建直径
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="color">颜色</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回直径</returns>
    Task<Diameter> CreateAsync(int size, string color, string? remark, CancellationToken cancellationToken = default);
}
