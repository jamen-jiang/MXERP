using MXERP.Domain.Common.Enums;
using MXERP.Domain.Departments;
using MXERP.Domain.Employees;
using MXERP.Domain.RetailOrderWorkSteps;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.ExternalRetailOrderTrackings;

/// <summary>
/// 外部散镜订单跟踪领域服务
/// </summary>
[ScopedService]
public class ExternalRetailOrderTrackingDomainService : IExternalRetailOrderTrackingDomainService
{
    private readonly IExternalRetailOrderTrackingRepository _externalRetailOrderTrackingRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRetailOrderWorkStepRepository _retailOrderWorkStepRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="externalRetailOrderTrackingRepository">外部散镜订单跟踪仓储</param>
    /// <param name="employeeRepository">员工仓储</param>
    /// <param name="departmentRepository">部门仓储</param>
    /// <param name="retailOrderWorkStepRepository">散镜订单工作步骤仓储</param>
    public ExternalRetailOrderTrackingDomainService(IExternalRetailOrderTrackingRepository externalRetailOrderTrackingRepository, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IRetailOrderWorkStepRepository retailOrderWorkStepRepository)
    {
        _externalRetailOrderTrackingRepository = externalRetailOrderTrackingRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _retailOrderWorkStepRepository = retailOrderWorkStepRepository;
    }

    /// <inheritdoc />
    public async Task<ExternalRetailOrderTracking?> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, CancellationToken cancellationToken = default)
    {
        if (number.Length != 16)
        {
            throw new UserFriendlyException("外部散镜订单号只能是16位!");
        }

        Employee? employee = await _employeeRepository.FindAsync(employeeId, true, cancellationToken);
        if (employee is null)
        {
            throw new UserFriendlyException("外部散镜订单" + number + "员工不存在!");
        }

        Department? outDepartment = await _departmentRepository.FindAsync(employee.DepartmentId, true, cancellationToken);
        if (outDepartment is null)
        {
            throw new UserFriendlyException("外部散镜订单" + number + "员工部门不存在!");
        }

        Department? inDepartment = await _departmentRepository.FindAsync(inDepartmentId, true, cancellationToken);
        if (inDepartment is null)
        {
            throw new UserFriendlyException("外部散镜订单" + number + "当前用户关联的部门不存在!");
        }

        if (workStepId is not null && string.IsNullOrWhiteSpace(reason))
        {
            RetailOrderWorkStep? workStep = await _retailOrderWorkStepRepository.FindAsync(workStepId.Value, true, cancellationToken);
            if (workStep is null)
            {
                throw new UserFriendlyException("外部散镜订单" + number + "工序不存在!");
            }

            ExternalRetailOrderTracking? exist = await _externalRetailOrderTrackingRepository.GetByNumberAndWorkStepIdAsync(number, workStepId.Value, cancellationToken);
            if (exist is not null)
            {
                throw new UserFriendlyException("外部散镜订单" + number + "工序 " + workStep.Name + " 重复录入!");
            }

            if (workStep.DepartmentId != employee.DepartmentId)
            {
                throw new UserFriendlyException("外部散镜订单" + number + "不允许跨部门录入工序!");
            }

            List<ExternalRetailOrderTracking> trackings = await _externalRetailOrderTrackingRepository.GetListByNumberAsync(number, cancellationToken);
            List<RetailOrderWorkStep> workSteps = await GetEffectiveWorkStepsAsync(cancellationToken);
            List<RetailOrderWorkStep> aboveWorkSteps = workSteps.Where(x => x.Sequence < workStep.Sequence).OrderBy(x => x.Sequence).ToList();
            foreach (RetailOrderWorkStep above in aboveWorkSteps)
            {
                bool aboveDone = trackings.Any(x => x.WorkStepId == above.Id);
                if (!aboveDone)
                {
                    throw new UserFriendlyException("外部散镜订单" + number + "不允许跳过工序 " + above.Name + " 录入!");
                }
            }

            return new ExternalRetailOrderTracking(number, workStep.Id, employeeId, outDepartment.Id, inDepartment.Id);
        }

        if (workStepId is null && !string.IsNullOrWhiteSpace(reason))
        {
            return new ExternalRetailOrderTracking(number, reason, employeeId, outDepartment.Id, inDepartment.Id);
        }

        return null;
    }

    /// <summary>
    /// 获取有效工序列表（排除「送仓库」「接受订单」）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>有效工序列表（按序列升序）</returns>
    private async Task<List<RetailOrderWorkStep>> GetEffectiveWorkStepsAsync(CancellationToken cancellationToken)
    {
        List<RetailOrderWorkStep> workSteps = await _retailOrderWorkStepRepository.GetAllOrderBySequenceAsync(cancellationToken);
        return workSteps.Where(x => x.Code != RetailOrderWorkStepEnum.SongCangKu && x.Code != RetailOrderWorkStepEnum.JieShouDingDan).ToList();
    }
}
