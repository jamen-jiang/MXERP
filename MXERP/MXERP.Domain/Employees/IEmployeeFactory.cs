namespace MXERP.Domain.Employees;

/// <summary>
/// 员工工厂接口
/// </summary>
public interface IEmployeeFactory
{
    /// <summary>
    /// 创建员工(布尔性别)
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别(true=女)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回员工</returns>
    Task<Employee> CreateAsync(string number, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建员工(字符串性别)
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别字符串(可空)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回员工</returns>
    Task<Employee> CreateAsync(string number, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);
}
