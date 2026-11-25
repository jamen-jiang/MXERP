using MXERP.Domain.Common.Enums;
using MXERP.Domain.Companies;
using MXERP.Domain.CustomersSpus;
using MXERP.Domain.Departments;
using MXERP.Domain.Employees;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.RetailOrderWorkSteps;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.Spus;
using MXERP.Domain.Stores;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单领域服务
/// </summary>
[ScopedService]
public class RetailOrderDomainService : IRetailOrderDomainService
{
    private readonly IRetailOrderRepository _retailOrderRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly ICustomerSpuRepository _customerSpuRepository;
    private readonly IRetailOrderWorkStepRepository _retailOrderWorkStepRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ISalesSpuRepository _salesSpuRepository;
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    private readonly IRetailOrderFactory _retailOrderFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="retailOrderRepository">散镜订单仓储</param>
    /// <param name="storeRepository">门店仓储</param>
    /// <param name="companyRepository">公司仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="customerSpuRepository">客人产品仓储</param>
    /// <param name="retailOrderWorkStepRepository">散镜订单工序仓储</param>
    /// <param name="employeeRepository">员工仓储</param>
    /// <param name="departmentRepository">部门仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="retailOrderFactory">散镜订单工厂</param>
    public RetailOrderDomainService(
        IRetailOrderRepository retailOrderRepository,
        IStoreRepository storeRepository,
        ICompanyRepository companyRepository,
        ISpuRepository spuRepository,
        ICustomerSpuRepository customerSpuRepository,
        IRetailOrderWorkStepRepository retailOrderWorkStepRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        ISalesSpuRepository salesSpuRepository,
        ILeaveStockBatchRepository leaveStockBatchRepository,
        IRetailOrderFactory retailOrderFactory)
    {
        _retailOrderRepository = retailOrderRepository;
        _storeRepository = storeRepository;
        _companyRepository = companyRepository;
        _spuRepository = spuRepository;
        _customerSpuRepository = customerSpuRepository;
        _retailOrderWorkStepRepository = retailOrderWorkStepRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _salesSpuRepository = salesSpuRepository;
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _retailOrderFactory = retailOrderFactory;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> CreateAsync(RetailOrderData data, long userId, string userName, CancellationToken cancellationToken = default)
    {
        string? customerPONumber = data.CustomerPONumber;

        MasterData masterData = await LoadMasterDataAsync(
            data.StoreId,
            data.CompanyId,
            data.SalesSpuIdR,
            data.CustomerSpuIdR,
            data.SpuIdR,
            data.SalesSpuIdL,
            data.CustomerSpuIdL,
            data.SpuIdL,
            cancellationToken);

        if (string.Equals(masterData.Company.Code, "AS500", StringComparison.OrdinalIgnoreCase))
        {
            customerPONumber = data.CustomerOrderNumber;
        }

        await CheckOrderNumberUniqueAsync(data.CustomerOrderNumber, customerPONumber, null, cancellationToken);

        await CheckPoNumberUniqueForSpecialCompanyAsync(masterData.Company, customerPONumber, null, cancellationToken);

        RetailOrder model = await BuildRetailOrderAsync(data, customerPONumber, masterData, null, cancellationToken);

        await AddTrackingAsync(model, RetailOrderWorkStepEnum.JieShouDingDan, cancellationToken);
        model.Check(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> ModifyAsync(RetailOrderData data, CancellationToken cancellationToken = default)
    {
        if (data.Id is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        RetailOrder? model = await _retailOrderRepository.FindAsync(data.Id.Value, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        await CheckOrderNumberUniqueAsync(data.CustomerOrderNumber, data.CustomerPONumber, data.Id, cancellationToken);

        MasterData masterData = await LoadMasterDataAsync(
            model.StoreId,
            model.CompanyId,
            data.SalesSpuIdR,
            data.CustomerSpuIdR,
            data.SpuIdR,
            data.SalesSpuIdL,
            data.CustomerSpuIdL,
            data.SpuIdL,
            cancellationToken);

        await CheckPoNumberUniqueForSpecialCompanyAsync(masterData.Company, data.CustomerPONumber, data.Id, cancellationToken);

        return await BuildRetailOrderAsync(data, data.CustomerPONumber, masterData, model, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RetailOrder> UnCheckAsync(long id, string? remark, CancellationToken cancellationToken = default)
    {
        RetailOrder? model = await _retailOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        bool isExistLeaveStockBatch = await _leaveStockBatchRepository.IsExistByRelationAsync(BusinessOrderTypeEnum.RetailOrder, model.Id, cancellationToken);
        if (isExistLeaveStockBatch)
        {
            throw new UserFriendlyException("散镜订单" + model.Number + "已生成出库批次不允许弃审!");
        }

        model.UnCheck(remark);
        return model;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> FinishAsync(long id, long userId, string userName, CancellationToken cancellationToken = default)
    {
        RetailOrder? model = await _retailOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单" + id + "不存在!");
        }

        List<RetailOrderWorkStep> workSteps = await GetEffectiveWorkStepsAsync(cancellationToken);
        long? lastWorkStepId = null;
        int maxSequence = int.MinValue;
        foreach (RetailOrderWorkStep workStep in workSteps)
        {
            if (workStep.Sequence > maxSequence)
            {
                maxSequence = workStep.Sequence;
                lastWorkStepId = workStep.Id;
            }
        }

        bool hasLast = false;
        foreach (RetailOrderTracking tracking in model.RetailOrderTrackings)
        {
            if (tracking.WorkStepId == lastWorkStepId)
            {
                hasLast = true;
                break;
            }
        }

        if (!hasLast)
        {
            throw new UserFriendlyException("散镜订单" + model.Number + "工序未全部录入!");
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> UnFinishAsync(long id, string? remark, CancellationToken cancellationToken = default)
    {
        RetailOrder? model = await _retailOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        model.UnFinish(remark);
        return model;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> CancelAsync(long id, string? remark, long userId, string userName, CancellationToken cancellationToken = default)
    {
        RetailOrder? model = await _retailOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        await AddTrackingAsync(model, RetailOrderWorkStepEnum.QuXiao, cancellationToken);
        model.Cancel(userId, userName, remark);
        return model;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, long userId, string userName, CancellationToken cancellationToken = default)
    {
        RetailOrder? model = await _retailOrderRepository.GetByNumberAsync(number, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单" + number + "不存在!");
        }

        Employee? employee = await _employeeRepository.FindAsync(employeeId, true, cancellationToken);
        if (employee is null)
        {
            throw new UserFriendlyException("散镜订单" + number + "员工不存在!");
        }

        Department? outDepartment = await _departmentRepository.FindAsync(employee.DepartmentId, true, cancellationToken);
        if (outDepartment is null)
        {
            throw new UserFriendlyException("散镜订单" + number + "员工部门不存在!");
        }

        if (workStepId is not null && reason.IsNullOrWhiteSpace())
        {
            RetailOrderWorkStep? workStep = await _retailOrderWorkStepRepository.FindAsync(workStepId.Value, true, cancellationToken);
            if (workStep is null)
            {
                throw new UserFriendlyException("散镜订单" + number + "工序不存在!");
            }

            bool isDuplicate = false;
            foreach (RetailOrderTracking tracking in model.RetailOrderTrackings)
            {
                if (tracking.WorkStepId == workStepId)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (isDuplicate)
            {
                throw new UserFriendlyException("散镜订单" + number + "工序 " + workStep.Name + " 重复录入!");
            }

            if (workStep.DepartmentId != employee.DepartmentId)
            {
                throw new UserFriendlyException("散镜订单" + number + "不允许跨部门录入工序!");
            }

            List<RetailOrderWorkStep> workSteps = await GetEffectiveWorkStepsAsync(cancellationToken);
            List<RetailOrderWorkStep> aboveWorkSteps = new List<RetailOrderWorkStep>();
            foreach (RetailOrderWorkStep item in workSteps)
            {
                if (item.Sequence < workStep.Sequence)
                {
                    aboveWorkSteps.Add(item);
                }
            }

            aboveWorkSteps.Sort((x, y) => x.Sequence.CompareTo(y.Sequence));

            foreach (RetailOrderWorkStep above in aboveWorkSteps)
            {
                bool aboveDone = false;
                foreach (RetailOrderTracking tracking in model.RetailOrderTrackings)
                {
                    if (tracking.WorkStepId == above.Id)
                    {
                        aboveDone = true;
                        break;
                    }
                }

                if (!aboveDone)
                {
                    throw new UserFriendlyException("散镜订单" + number + "不允许跳过工序 " + above.Name + " 录入!");
                }
            }

            model.AddTracking(workStep.Id, employee.Id, outDepartment.Id, inDepartmentId);

            long? lastWorkStepId = null;
            int maxSequence = int.MinValue;
            foreach (RetailOrderWorkStep item in workSteps)
            {
                if (item.Sequence > maxSequence)
                {
                    maxSequence = item.Sequence;
                    lastWorkStepId = item.Id;
                }
            }

            if (workStep.Id == lastWorkStepId)
            {
                model.Finish(userId, userName);
            }
        }
        else if (workStepId is null && !reason.IsNullOrWhiteSpace())
        {
            model.AddTracking(reason, employee.Id, outDepartment.Id, inDepartmentId);
        }

        return model;
    }

    private sealed record MasterData(
        Store Store,
        Company Company,
        SalesSpu SalesSpuR,
        CustomerSpu? CustomerSpuR,
        Spu SpuR,
        SalesSpu SalesSpuL,
        CustomerSpu? CustomerSpuL,
        Spu SpuL);

    private async Task<MasterData> LoadMasterDataAsync(
        long storeId,
        long companyId,
        long salesSpuIdR,
        long? customerSpuIdR,
        long spuIdR,
        long salesSpuIdL,
        long? customerSpuIdL,
        long spuIdL,
        CancellationToken cancellationToken)
    {
        Store? store = await _storeRepository.FindAsync(storeId, true, cancellationToken);
        if (store is null)
        {
            throw new UserFriendlyException("门店" + storeId + "不存在!");
        }

        Company? company = await _companyRepository.FindAsync(companyId, true, cancellationToken);
        if (company is null)
        {
            throw new UserFriendlyException("公司" + companyId + "不存在!");
        }

        SalesSpu? salesSpuR = await _salesSpuRepository.FindAsync(salesSpuIdR, true, cancellationToken);
        if (salesSpuR is null)
        {
            throw new UserFriendlyException("销售产品R " + salesSpuIdR + "不存在!");
        }

        SalesSpu? salesSpuL = await _salesSpuRepository.FindAsync(salesSpuIdL, true, cancellationToken);
        if (salesSpuL is null)
        {
            throw new UserFriendlyException("销售产品L " + salesSpuIdL + "不存在!");
        }

        CustomerSpu? customerSpuR = null;
        if (customerSpuIdR is not null)
        {
            customerSpuR = await _customerSpuRepository.FindAsync(customerSpuIdR.Value, true, cancellationToken);
            if (customerSpuR is null)
            {
                throw new UserFriendlyException("客人产品R " + customerSpuIdR + "不存在!");
            }
        }

        CustomerSpu? customerSpuL = null;
        if (customerSpuIdL is not null)
        {
            customerSpuL = await _customerSpuRepository.FindAsync(customerSpuIdL.Value, true, cancellationToken);
            if (customerSpuL is null)
            {
                throw new UserFriendlyException("客人产品L " + customerSpuIdL + "不存在!");
            }
        }

        Spu? spuR = await _spuRepository.FindAsync(spuIdR, true, cancellationToken);
        if (spuR is null)
        {
            throw new UserFriendlyException("产品R " + spuIdR + "不存在!");
        }

        Spu? spuL = await _spuRepository.FindAsync(spuIdL, true, cancellationToken);
        if (spuL is null)
        {
            throw new UserFriendlyException("产品L " + spuIdL + "不存在!");
        }

        return new MasterData(store, company, salesSpuR, customerSpuR, spuR, salesSpuL, customerSpuL, spuL);
    }

    private async Task CheckOrderNumberUniqueAsync(string customerOrderNumber, string? customerPONumber, long? id, CancellationToken cancellationToken)
    {
        bool isExist = await _retailOrderRepository.IsExistByCustomerOrderNumberAndCustomerPONumberExcludeIdAsync(customerOrderNumber, customerPONumber, id, cancellationToken);
        if (isExist)
        {
            string poSuffix = string.Empty;
            if (!customerPONumber.IsNullOrWhiteSpace())
            {
                poSuffix = "客户PO号" + customerPONumber;
            }

            throw new UserFriendlyException("客户单号" + customerOrderNumber + poSuffix + "已存在!");
        }
    }

    private async Task CheckPoNumberUniqueForSpecialCompanyAsync(Company company, string? customerPONumber, long? id, CancellationToken cancellationToken)
    {
        bool isSpecialCompany = string.Equals(company.Code, "SMD", StringComparison.OrdinalIgnoreCase)
            || string.Equals(company.Code, "XM001", StringComparison.OrdinalIgnoreCase);
        if (!isSpecialCompany || customerPONumber.IsNullOrWhiteSpace())
        {
            return;
        }

        bool isExist = await _retailOrderRepository.IsExistByCustomerPONumberExcludeIdAsync(customerPONumber, id, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("客户PO号" + customerPONumber + "已存在!");
        }
    }

    private async Task<RetailOrder> BuildRetailOrderAsync(RetailOrderData data, string? customerPONumber, MasterData masterData, RetailOrder? model, CancellationToken cancellationToken)
    {
        string? outArea = GetOutArea(data.OutArea, masterData.Company.Code, data.CustomerOrderNumber);

        if (model is null)
        {
            model = await _retailOrderFactory.CreateAsync(
                masterData.Store.Id,
                masterData.Company.Id,
                outArea,
                data.SequenceNumber,
                data.HkNumber,
                data.CustomerOrderNumber,
                customerPONumber,
                data.DocumentaryBarCode,
                data.OutGood,
                data.OutGoodTime,
                data.Material,
                data.Coating,
                data.TintingColor,
                data.Mirror,
                data.TintingCode,
                data.TintingName,
                data.Uv,
                data.Hard,
                data.Polish,
                data.Auro,
                data.Edge,
                data.SuperHole,
                data.Special,
                data.SpecialCode,
                data.ZuanKong,
                data.Frame,
                data.Remark,
                cancellationToken);
        }
        else
        {
            model.Modify(
                outArea,
                data.SequenceNumber,
                data.HkNumber,
                data.CustomerOrderNumber,
                data.CustomerPONumber,
                data.DocumentaryBarCode,
                data.OutGood,
                data.OutGoodTime,
                data.Material,
                data.Coating,
                data.TintingColor,
                data.Mirror,
                data.TintingCode,
                data.TintingName,
                data.Uv,
                data.Hard,
                data.Polish,
                data.Auro,
                data.Edge,
                data.SuperHole,
                data.Special,
                data.SpecialCode,
                data.ZuanKong,
                data.Frame,
                data.Remark);
        }

        AddItem(
            model,
            EyeTypeEnum.R,
            masterData.SalesSpuR,
            masterData.CustomerSpuR,
            masterData.SpuR,
            data.WarehouseIdR,
            data.SphR,
            data.CylR,
            data.AddR,
            data.DiameterR,
            data.AxisR,
            data.PdR,
            data.PhR,
            data.BaseR,
            data.CenterThicknessR,
            data.KnifeSideR,
            data.DecenterR,
            data.PrismR,
            data.QtyR);

        AddItem(
            model,
            EyeTypeEnum.L,
            masterData.SalesSpuL,
            masterData.CustomerSpuL,
            masterData.SpuL,
            data.WarehouseIdL,
            data.SphL,
            data.CylL,
            data.AddL,
            data.DiameterL,
            data.AxisL,
            data.PdL,
            data.PhL,
            data.BaseL,
            data.CenterThicknessL,
            data.KnifeSideL,
            data.DecenterL,
            data.PrismL,
            data.QtyL);

        return model;
    }

    private static void AddItem(
        RetailOrder model,
        EyeTypeEnum eyeType,
        SalesSpu salesSpu,
        CustomerSpu? customerSpu,
        Spu spu,
        long warehouseId,
        int sph,
        int cyl,
        int? add,
        int? diameter,
        int? axis,
        double? pd,
        double? ph,
        string? baseCurve,
        double? centerThickness,
        bool? knifeSide,
        string? decenter,
        string? prism,
        double qty)
    {
        long? customerSpuId = null;
        string? customerSpuCode = null;
        if (customerSpu is not null)
        {
            customerSpuId = customerSpu.Id;
            customerSpuCode = customerSpu.Code;
        }

        model.AddOrModifyItem(
            eyeType,
            salesSpu.Id,
            salesSpu.Code,
            customerSpuId,
            customerSpuCode,
            spu.Id,
            warehouseId,
            sph,
            cyl,
            add,
            diameter,
            axis,
            pd,
            ph,
            baseCurve,
            centerThickness,
            knifeSide,
            decenter,
            prism,
            qty);
    }

    private static string? GetOutArea(string? outArea, string? companyCode, string customerOrderNumber)
    {
        if (!outArea.IsNullOrWhiteSpace())
        {
            return outArea;
        }

        if (!companyCode.IsNullOrWhiteSpace())
        {
            string upperCode = companyCode.ToUpperInvariant();
            if (upperCode.Contains("HK", StringComparison.Ordinal))
            {
                return "HK";
            }

            if (upperCode.Contains("EGG", StringComparison.Ordinal))
            {
                return "061";
            }
        }

        if (customerOrderNumber.Length >= 2)
        {
            return customerOrderNumber.Substring(0, 2);
        }

        return outArea;
    }

    private async Task<List<RetailOrderWorkStep>> GetEffectiveWorkStepsAsync(CancellationToken cancellationToken)
    {
        List<RetailOrderWorkStep> workSteps = await _retailOrderWorkStepRepository.GetAllOrderBySequenceAsync(cancellationToken);
        List<RetailOrderWorkStep> result = new List<RetailOrderWorkStep>();
        foreach (RetailOrderWorkStep workStep in workSteps)
        {
            if (workStep.Code == RetailOrderWorkStepEnum.JieShouDingDan.GetDescription())
            {
                continue;
            }

            if (workStep.Code == RetailOrderWorkStepEnum.QuXiao.GetDescription())
            {
                continue;
            }

            result.Add(workStep);
        }

        return result;
    }

    private async Task AddTrackingAsync(RetailOrder retailOrder, RetailOrderWorkStepEnum workStepCode, CancellationToken cancellationToken)
    {
        RetailOrderWorkStep? workStep = await _retailOrderWorkStepRepository.GetByCodeAsync(workStepCode.GetDescription(), cancellationToken);
        if (workStep is null)
        {
            throw new UserFriendlyException("工序" + workStepCode + "不存在!");
        }

        retailOrder.AddTracking(workStep.Id, 0L, 0L, 0L);
    }
}
