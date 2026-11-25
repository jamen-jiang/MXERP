namespace MXERP.Domain.Warehouses;

public interface IWarehouseFactory
{
    /// <summary>
    /// 创建仓库
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="address">地址</param>
    /// <param name="manageManName">管理员</param>
    /// <param name="telephone">电话</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回仓库</returns>
    Task<Warehouse> CreateAsync(string code, string name, string? address, string? manageManName, string? telephone, string? remark, CancellationToken cancellationToken = default);
}
