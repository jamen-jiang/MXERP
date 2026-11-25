using MXERP.Domain.Employees;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.WorkSteps;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次领域服务
/// </summary>
[ScopedService]
public class LeaveStockBatchDomainService : ILeaveStockBatchDomainService
{
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    private readonly ISalesSpuRepository _salesSpuRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWorkStepRepository _workStepRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="employeeRepository">员工仓储</param>
    /// <param name="workStepRepository">工序仓储</param>
    public LeaveStockBatchDomainService(ILeaveStockBatchRepository leaveStockBatchRepository, ISalesSpuRepository salesSpuRepository, IEmployeeRepository employeeRepository, IWorkStepRepository workStepRepository)
    {
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _salesSpuRepository = salesSpuRepository;
        _employeeRepository = employeeRepository;
        _workStepRepository = workStepRepository;
    }

    /// <inheritdoc />
    public async Task<LeaveStockBatch> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, CancellationToken cancellationToken = default)
    {
        LeaveStockBatch? leaveStockBatch = await _leaveStockBatchRepository.GetByNumberAsync(number, cancellationToken);
        if (leaveStockBatch is null)
        {
            throw new UserFriendlyException("出库批次" + number + "不存在!");
        }

        if (leaveStockBatch.GetLeaveStockBatchWorkSteps().Count == 0)
        {
            throw new UserFriendlyException("出库批次" + number + "还未配置工序列表!");
        }

        Employee? employee = await _employeeRepository.FindAsync(employeeId, true, cancellationToken);
        if (employee is null)
        {
            throw new UserFriendlyException("出库批次" + number + "员工不存在!");
        }

        if (workStepId is not null && string.IsNullOrWhiteSpace(reason))
        {
            List<long> workStepIds = leaveStockBatch.GetLeaveStockBatchWorkSteps().Select(x => x.WorkStepId).ToList();
            List<WorkStep> workSteps = await _workStepRepository.GetListByIdsAsync(workStepIds, cancellationToken);
            WorkStep? workStep = workSteps.FirstOrDefault(x => x.Id == workStepId.Value);
            if (workStep is null)
            {
                throw new UserFriendlyException("出库批次" + number + "工序不存在!");
            }

            LeaveStockBatchTracking? leaveStockBatchTracking = leaveStockBatch.GetLeaveStockBatchTrackings().FirstOrDefault(x => x.WorkStepId == workStepId.Value);
            if (leaveStockBatchTracking is not null)
            {
                throw new UserFriendlyException("出库批次" + number + "工序 " + workStep.Name + " 重复录入!");
            }

            if (workStep.DepartmentId != employee.DepartmentId)
            {
                throw new UserFriendlyException("出库批次" + number + "不允许跨部门录入工序!");
            }

            List<WorkStep> aboveWorkSteps = workSteps.Where(x => x.Sequence < workStep.Sequence).OrderBy(x => x.Sequence).ToList();
            foreach (WorkStep aboveWorkStep in aboveWorkSteps)
            {
                LeaveStockBatchTracking? aboveOrderTracking = leaveStockBatch.GetLeaveStockBatchTrackings().FirstOrDefault(x => x.WorkStepId == aboveWorkStep.Id);
                if (aboveOrderTracking is null)
                {
                    throw new UserFriendlyException("出库批次" + number + "不允许跳过工序 " + aboveWorkStep.Name + " 录入!");
                }
            }

            leaveStockBatch.AddTracking(workStep.Id, employeeId, employee.DepartmentId, inDepartmentId);
        }
        else if (workStepId is null && !string.IsNullOrWhiteSpace(reason))
        {
            leaveStockBatch.AddTracking(reason, employeeId, employee.DepartmentId, inDepartmentId);
        }

        return leaveStockBatch;
    }

    /// <inheritdoc />
    public async Task<LeaveStockBatch> SyncLeaveStockBatchWorkStepsAsync(long id, CancellationToken cancellationToken = default)
    {
        LeaveStockBatch? model = await _leaveStockBatchRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("出库批次" + id + "不存在!");
        }

        LeaveStockBatchItem? first = model.GetLeaveStockBatchItems().FirstOrDefault();
        if (first is null)
        {
            return model;
        }

        long? salesSpuId = first.SalesSpuId;
        if (salesSpuId is not null)
        {
            SalesSpu? salesSpu = await _salesSpuRepository.FindAsync(salesSpuId.Value, true, cancellationToken);
            if (salesSpu is null)
            {
                throw new UserFriendlyException("出库批次" + model.Number + "销售产品" + salesSpuId + "不存在!");
            }

            await SyncLeaveStockBatchWorkStepsAsync(model, salesSpu.GetSalesSpuWorkSteps().Select(x => x.WorkStepId), cancellationToken);
        }

        return model;
    }

    /// <inheritdoc />
    public async Task SyncLeaveStockBatchWorkStepsAsync(LeaveStockBatch leaveStockBatch, IEnumerable<long>? workStepIds, CancellationToken cancellationToken = default)
    {
        List<long> finalWorkStepIds = workStepIds is null ? [] : workStepIds.ToList();

        List<LeaveStockBatchWorkStep> toRemoves = leaveStockBatch.GetLeaveStockBatchWorkSteps().Where(a => !finalWorkStepIds.Contains(a.WorkStepId)).ToList();
        foreach (LeaveStockBatchWorkStep item in toRemoves)
        {
            leaveStockBatch.DeleteWorkStep(item.Id);
        }

        foreach (long workStepId in finalWorkStepIds)
        {
            WorkStep? workStep = await _workStepRepository.FindAsync(workStepId, true, cancellationToken);
            if (workStep is null)
            {
                throw new UserFriendlyException("工序 " + workStepId + "不存在!");
            }

            leaveStockBatch.AddWorkStep(workStepId);
        }
    }
}
