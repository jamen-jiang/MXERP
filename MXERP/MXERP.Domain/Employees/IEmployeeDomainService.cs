namespace MXERP.Domain.Employees;

/// <summary>
/// 员工领域服务接口
/// </summary>
public interface IEmployeeDomainService
{
    /// <summary>
    /// 修改员工(按Id查询后修改, 布尔性别)
    /// </summary>
    /// <param name="id">员工Id</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别(true=女)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回员工</returns>
    Task<Employee> ModifyAsync(long id, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改员工(按Id查询后修改, 字符串性别)
    /// </summary>
    /// <param name="id">员工Id</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别字符串(可空)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回员工</returns>
    Task<Employee> ModifyAsync(long id, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改员工(使用已有聚合, 布尔性别)
    /// </summary>
    /// <param name="employee">员工聚合</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别(true=女)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Employee employee, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改员工(使用已有聚合, 字符串性别)
    /// </summary>
    /// <param name="employee">员工聚合</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别字符串(可空)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Employee employee, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成: 员工存在则修改, 不存在则创建
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
    Task<Employee> ImportGenerateAsync(string number, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default);
}
