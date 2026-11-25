using Snow.Domain.Repositories;

namespace MXERP.Domain.Employees;

/// <summary>
/// 员工仓储接口
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// 按员工号查询
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>员工, 不存在返回 null</returns>
    Task<Employee?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 员工号是否已存在
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 员工条码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="barCode">员工条码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByBarCodeExcludeIdAsync(string barCode, long? id = null, CancellationToken cancellationToken = default);
}
