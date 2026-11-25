using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Employees;

/// <summary>
/// 员工(对应 Java Employee)
/// </summary>
public class Employee : AuditedAggregateRoot
{
    /// <summary>
    /// 员工姓名
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 员工号
    /// </summary>
    public string Number { get; private set; } = string.Empty;
    /// <summary>
    /// 员工条码
    /// </summary>
    public string BarCode { get; private set; } = string.Empty;
    /// <summary>
    /// 性别(true=女)
    /// </summary>
    public bool Gender { get; private set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; private set; }
    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Employee()
    {
    }

    /// <summary>
    /// 构造器(布尔性别)
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别(true=女)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    internal Employee(string number, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark)
    {
        if (number.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("员工号不能为空");
        }

        if (barCode.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("员工条码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("员工姓名不能为空");
        }

        Number = number.Trim();
        BarCode = barCode.Trim();
        Name = name.Trim();
        DepartmentId = departmentId;
        Gender = gender;
        IsEnable = isEnable;
        Remark = remark;
    }

    /// <summary>
    /// 构造器(字符串性别, "女"为 true, 其他为 false)
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别字符串(可空)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    internal Employee(string number, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark)
        : this(number, barCode, name, departmentId, "女" == gender, isEnable, remark)
    {
    }

    /// <summary>
    /// 修改员工(布尔性别)
    /// </summary>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别(true=女)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark)
    {
        if (barCode.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("员工条码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("员工姓名不能为空");
        }

        BarCode = barCode.Trim();
        Name = name.Trim();
        DepartmentId = departmentId;
        Gender = gender;
        IsEnable = isEnable;
        Remark = remark;
    }

    /// <summary>
    /// 修改员工(字符串性别, "女"为 true, 其他为 false)
    /// </summary>
    /// <param name="barCode">员工条码</param>
    /// <param name="name">员工姓名</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="gender">性别字符串(可空)</param>
    /// <param name="isEnable">是否启用</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark)
    {
        Modify(barCode, name, departmentId, "女" == gender, isEnable, remark);
    }
}
