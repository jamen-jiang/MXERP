using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Employees;

[ScopedService]
public class EmployeeFactory : IEmployeeFactory
{
    private readonly IEmployeeRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public EmployeeFactory(IEmployeeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Employee> CreateAsync(string number, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        await CheckUniqueAsync(number, barCode, cancellationToken);
        return new Employee(number, barCode, name, departmentId, gender, isEnable, remark);
    }

    /// <inheritdoc />
    public async Task<Employee> CreateAsync(string number, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        await CheckUniqueAsync(number, barCode, cancellationToken);
        return new Employee(number, barCode, name, departmentId, gender, isEnable, remark);
    }

    /// <summary>
    /// 校验员工号与员工条码唯一
    /// </summary>
    /// <param name="number">员工号</param>
    /// <param name="barCode">员工条码</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task CheckUniqueAsync(string number, string barCode, CancellationToken cancellationToken)
    {
        if (await _repository.IsExistByNumberAsync(number, cancellationToken))
        {
            throw new UserFriendlyException($"员工号{number}已存在!");
        }

        if (await _repository.IsExistByBarCodeExcludeIdAsync(barCode, null, cancellationToken))
        {
            throw new UserFriendlyException($"员工条码{barCode}已存在!");
        }
    }
}
