using MXERP.Domain.Employees;
using MXERP.Domain.WorkSteps;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单领域服务
/// </summary>
[ScopedService]
public class ExternalLeaveStockOrderDomainService : IExternalLeaveStockOrderDomainService
{
    /// <summary>
    /// 外部出库单仓储
    /// </summary>
    private readonly IExternalLeaveStockOrderRepository _externalLeaveStockOrderRepository;

    /// <summary>
    /// 员工仓储
    /// </summary>
    private readonly IEmployeeRepository _employeeRepository;

    /// <summary>
    /// 工序仓储
    /// </summary>
    private readonly IWorkStepRepository _workStepRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="externalLeaveStockOrderRepository">外部出库单仓储</param>
    /// <param name="employeeRepository">员工仓储</param>
    /// <param name="workStepRepository">工序仓储</param>
    public ExternalLeaveStockOrderDomainService(
        IExternalLeaveStockOrderRepository externalLeaveStockOrderRepository,
        IEmployeeRepository employeeRepository,
        IWorkStepRepository workStepRepository)
    {
        _externalLeaveStockOrderRepository = externalLeaveStockOrderRepository;
        _employeeRepository = employeeRepository;
        _workStepRepository = workStepRepository;
    }

    /// <inheritdoc />
    public async Task<ExternalLeaveStockOrder> ModifyAsync(
        long id,
        string customerOrderNumber,
        string? customerPONumber,
        long salesSpuId,
        string salesSpuCode,
        long spuId,
        int orderQty,
        int qty,
        bool songCangKu,
        bool faLiao,
        bool kouZhang,
        bool songBaoZhuang,
        bool etc,
        bool daMai,
        bool huangYou,
        bool daYinTiaoMa,
        bool huanJingDai,
        bool tieBeiTie,
        bool daBao,
        bool zhuangXiangMingXi,
        bool chuHuoJianCha,
        bool fengXiang,
        DateTime orderOn,
        DateTime? deliveryOn,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        ExternalLeaveStockOrder? order = await _externalLeaveStockOrderRepository.FindAsync(id, true, cancellationToken);
        if (order is null)
        {
            throw new UserFriendlyException("外部出库单" + id + "不存在!");
        }

        order.Modify(
            customerOrderNumber,
            customerPONumber,
            salesSpuId,
            salesSpuCode,
            spuId,
            orderQty,
            qty,
            songCangKu,
            faLiao,
            kouZhang,
            songBaoZhuang,
            etc,
            daMai,
            huangYou,
            daYinTiaoMa,
            huanJingDai,
            tieBeiTie,
            daBao,
            zhuangXiangMingXi,
            chuHuoJianCha,
            fengXiang,
            orderOn,
            deliveryOn,
            remark);

        return order;
    }

    /// <inheritdoc />
    public async Task<ExternalLeaveStockOrder> AddTrackingAsync(
        string number,
        long? workStepId,
        string? reason,
        long employeeId,
        long outDepartmentId,
        long inDepartmentId,
        long userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        ExternalLeaveStockOrder? order = await _externalLeaveStockOrderRepository.GetByNumberAsync(number, cancellationToken);
        if (order is null)
        {
            throw new UserFriendlyException("外部出库单" + number + "不存在!");
        }

        Employee? employee = await _employeeRepository.FindAsync(employeeId, true, cancellationToken);
        if (employee is null)
        {
            throw new UserFriendlyException("外部出库单" + number + "员工不存在!");
        }

        if (workStepId is not null && reason.IsNullOrWhiteSpace())
        {
            List<WorkStep> workSteps = await GetWorkStepsAsync(order, cancellationToken);
            WorkStep workStep = FindWorkStep(workSteps, number, workStepId.Value);

            bool duplicated = order.GetExternalLeaveStockOrderTrackings()
                .Any(x => x.WorkStepId == workStepId);
            if (duplicated)
            {
                throw new UserFriendlyException("外部出库单" + number + "工序 " + workStep.Name + " 重复录入!");
            }

            if (workStep.DepartmentId != employee.DepartmentId)
            {
                throw new UserFriendlyException("外部出库单" + number + "不允许跨部门录入工序!");
            }

            List<WorkStep> aboveWorkSteps = workSteps
                .Where(x => x.Sequence < workStep.Sequence)
                .OrderBy(x => x.Sequence)
                .ToList();
            foreach (WorkStep aboveWorkStep in aboveWorkSteps)
            {
                bool recorded = order.GetExternalLeaveStockOrderTrackings()
                    .Any(x => x.WorkStepId == aboveWorkStep.Id);
                if (!recorded)
                {
                    throw new UserFriendlyException("外部出库单" + number + "不允许跳过工序 " + aboveWorkStep.Name + " 录入!");
                }
            }

            order.AddTracking(workStep.Id, employeeId, outDepartmentId, inDepartmentId);

            WorkStep? lastWorkStep = workSteps.OrderByDescending(x => x.Sequence).FirstOrDefault();
            if (lastWorkStep is not null && lastWorkStep.Id == workStep.Id)
            {
                order.Finish(userId, userName);
            }
        }
        else if (workStepId is null && !reason.IsNullOrWhiteSpace())
        {
            order.AddTracking(reason, employeeId, outDepartmentId, inDepartmentId);
        }

        return order;
    }

    /// <summary>
    /// 按外部出库单勾选标志组装工序列表
    /// </summary>
    /// <param name="order">外部出库单</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    private async Task<List<WorkStep>> GetWorkStepsAsync(ExternalLeaveStockOrder order, CancellationToken cancellationToken)
    {
        List<WorkStep> workSteps = await _workStepRepository.GetAllOrderBySequenceAsync(cancellationToken);
        List<WorkStep> newWorkSteps = new List<WorkStep>();
        AddStep(newWorkSteps, workSteps, order.SongCangKu, ExternalLeaveStockOrderWorkStepEnum.SongCangKu);
        AddStep(newWorkSteps, workSteps, order.FaLiao, ExternalLeaveStockOrderWorkStepEnum.FaLiao);
        AddStep(newWorkSteps, workSteps, order.KouZhang, ExternalLeaveStockOrderWorkStepEnum.KouZhang);
        AddStep(newWorkSteps, workSteps, order.SongBaoZhuang, ExternalLeaveStockOrderWorkStepEnum.SongBaoZhuang);
        AddStep(newWorkSteps, workSteps, order.Etc, ExternalLeaveStockOrderWorkStepEnum.Etc);
        AddStep(newWorkSteps, workSteps, order.DaMai, ExternalLeaveStockOrderWorkStepEnum.DaMai);
        AddStep(newWorkSteps, workSteps, order.HuangYou, ExternalLeaveStockOrderWorkStepEnum.HuangYou);
        AddStep(newWorkSteps, workSteps, order.DaYinTiaoMa, ExternalLeaveStockOrderWorkStepEnum.DaYinTiaoMa);
        AddStep(newWorkSteps, workSteps, order.HuanJingDai, ExternalLeaveStockOrderWorkStepEnum.HuanJingDai);
        AddStep(newWorkSteps, workSteps, order.TieBeiTie, ExternalLeaveStockOrderWorkStepEnum.TieBeiTie);
        AddStep(newWorkSteps, workSteps, order.DaBao, ExternalLeaveStockOrderWorkStepEnum.DaBao);
        AddStep(newWorkSteps, workSteps, order.ZhuangXiangMingXi, ExternalLeaveStockOrderWorkStepEnum.ZhuangXiangMingXi);
        AddStep(newWorkSteps, workSteps, order.ChuHuoJianCha, ExternalLeaveStockOrderWorkStepEnum.ChuHuoJianCha);
        AddStep(newWorkSteps, workSteps, order.FengXiang, ExternalLeaveStockOrderWorkStepEnum.FengXiang);
        return newWorkSteps;
    }

    /// <summary>
    /// 添加工序
    /// </summary>
    /// <param name="newWorkSteps">目标工序列表</param>
    /// <param name="workSteps">全量工序列表</param>
    /// <param name="isExecute">是否执行</param>
    /// <param name="workStepCode">工序编码</param>
    private static void AddStep(List<WorkStep> newWorkSteps, List<WorkStep> workSteps, bool isExecute, string workStepCode)
    {
        WorkStep? workStep = workSteps.FirstOrDefault(x => x.Code == workStepCode);
        if (workStep is null)
        {
            throw new UserFriendlyException("工序:" + workStepCode + "不存在!");
        }

        if (isExecute)
        {
            newWorkSteps.Add(workStep);
        }
    }

    /// <summary>
    /// 在工序列表中查找指定工序
    /// </summary>
    /// <param name="workSteps">工序列表</param>
    /// <param name="number">外部出库单号</param>
    /// <param name="workStepId">工序Id</param>
    /// <returns>工序</returns>
    private static WorkStep FindWorkStep(List<WorkStep> workSteps, string number, long workStepId)
    {
        WorkStep? workStep = workSteps.FirstOrDefault(x => x.Id == workStepId);
        if (workStep is null)
        {
            throw new UserFriendlyException("外部出库单" + number + "工序不存在!");
        }

        return workStep;
    }
}
