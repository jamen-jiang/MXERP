using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Companies;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.Spus;
using MXERP.Domain.Warehouses;

namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项领域服务
/// </summary>
[ScopedService]
public class SalesSpuOtherAssignItemDomainService : ISalesSpuOtherAssignItemDomainService
{
    private readonly ISalesSpuOtherAssignItemFactory _salesSpuOtherAssignItemFactory;
    private readonly ISalesSpuOtherAssignItemRepository _salesSpuOtherAssignItemRepository;
    private readonly ISalesSpuRepository _salesSpuRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpuOtherAssignItemFactory">销售产品其他分配项工厂</param>
    /// <param name="salesSpuOtherAssignItemRepository">销售产品其他分配项仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="companyRepository">公司仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="warehouseRepository">仓库仓储</param>
    public SalesSpuOtherAssignItemDomainService(ISalesSpuOtherAssignItemFactory salesSpuOtherAssignItemFactory, ISalesSpuOtherAssignItemRepository salesSpuOtherAssignItemRepository, ISalesSpuRepository salesSpuRepository, ICompanyRepository companyRepository, ISpuRepository spuRepository, IWarehouseRepository warehouseRepository)
    {
        _salesSpuOtherAssignItemFactory = salesSpuOtherAssignItemFactory;
        _salesSpuOtherAssignItemRepository = salesSpuOtherAssignItemRepository;
        _salesSpuRepository = salesSpuRepository;
        _companyRepository = companyRepository;
        _spuRepository = spuRepository;
        _warehouseRepository = warehouseRepository;
    }

    /// <inheritdoc />
    public async Task<SalesSpuOtherAssignItem> ModifyAsync(long id, long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default)
    {
        SalesSpuOtherAssignItem? model = await _salesSpuOtherAssignItemRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("销售产品其他分配项不存在!");
        }

        await ModifyAsync(model, salesSpuId, coating, spuId, warehouseId, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(SalesSpuOtherAssignItem model, long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default)
    {
        if (await _salesSpuOtherAssignItemRepository.IsExistBySalesSpuIdAndCoatingExcludeIdAsync(salesSpuId, coating, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("销售产品其他分配项已存在");
        }

        model.Modify(salesSpuId, coating, spuId, warehouseId);
    }

    /// <inheritdoc />
    public async Task<SalesSpuOtherAssignItem> ImportGenerateAsync(string companyCode, string salesCode, string coating, string spuCode, string warehouseCode, CancellationToken cancellationToken = default)
    {
        List<string> errors = new List<string>();

        Company? company = await _companyRepository.GetByCodeAsync(companyCode, cancellationToken);
        SalesSpu? salesSpu = null;
        if (company == null)
        {
            errors.Add("公司" + companyCode + "不存在");
        }
        else
        {
            salesSpu = await _salesSpuRepository.GetByProductTypeAndCompanyIdAndCodeAsync(ProductTypeEnum.EndProduct, company.Id, salesCode, cancellationToken);
            if (salesSpu == null)
            {
                errors.Add("销售产品" + salesCode + "不存在");
            }
        }

        Spu? spu = await _spuRepository.GetByProductTypeAndCodeAsync(ProductTypeEnum.EndProduct, spuCode, cancellationToken);
        if (spu == null)
        {
            errors.Add("产品" + spuCode + "不存在");
        }

        Warehouse? warehouse = await _warehouseRepository.GetByCodeAsync(warehouseCode, cancellationToken);
        if (warehouse == null)
        {
            errors.Add("仓库" + warehouseCode + "不存在");
        }

        if (errors.Count > 0)
        {
            throw new UserFriendlyException(string.Join(",", errors));
        }

        SalesSpuOtherAssignItem? model = await _salesSpuOtherAssignItemRepository.GetBySalesSpuIdAndCoatingAsync(salesSpu!.Id, coating, cancellationToken);
        if (model == null)
        {
            model = await _salesSpuOtherAssignItemFactory.GenerateAsync(salesSpu.Id, coating, spu!.Id, warehouse!.Id, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, salesSpu.Id, coating, spu!.Id, warehouse!.Id, cancellationToken);
        }

        return model;
    }
}
