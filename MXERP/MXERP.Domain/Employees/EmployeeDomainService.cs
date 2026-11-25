using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Employees;

[ScopedService]
public class EmployeeDomainService : IEmployeeDomainService
{
    private readonly IEmployeeFactory _factory;
    private readonly IEmployeeRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">员工工厂</param>
    /// <param name="repository">仓储</param>
    public EmployeeDomainService(IEmployeeFactory factory, IEmployeeRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Employee> ModifyAsync(long id, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        Employee? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("员工不存在");
        }

        await ModifyAsync(model, barCode, name, departmentId, gender, isEnable, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task<Employee> ModifyAsync(long id, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        Employee? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("员工不存在");
        }

        await ModifyAsync(model, barCode, name, departmentId, gender, isEnable, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Employee employee, string barCode, string name, long departmentId, bool gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByBarCodeExcludeIdAsync(barCode, employee.Id, cancellationToken))
        {
            throw new UserFriendlyException($"员工条码{barCode}已存在!");
        }

        employee.Modify(barCode, name, departmentId, gender, isEnable, remark);
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Employee employee, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByBarCodeExcludeIdAsync(barCode, employee.Id, cancellationToken))
        {
            throw new UserFriendlyException($"员工条码{barCode}已存在!");
        }

        employee.Modify(barCode, name, departmentId, gender, isEnable, remark);
    }

    /// <inheritdoc />
    public async Task<Employee> ImportGenerateAsync(string number, string barCode, string name, long departmentId, string? gender, bool isEnable, string? remark, CancellationToken cancellationToken = default)
    {
        Employee? model = await _repository.GetByNumberAsync(number, cancellationToken);
        if (model is null)
        {
            model = await _factory.CreateAsync(number, barCode, name, departmentId, gender, isEnable, remark, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, barCode, name, departmentId, gender, isEnable, remark, cancellationToken);
        }

        return model;
    }
}
