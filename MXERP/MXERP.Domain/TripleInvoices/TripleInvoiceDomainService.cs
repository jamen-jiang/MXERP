using MXERP.Domain.Common.Enums;
using MXERP.Domain.CustomerInvoiceTitleNumbers;
using MXERP.Domain.RetailOrders;
using MXERP.Domain.Stores;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票领域服务
/// </summary>
[ScopedService]
public class TripleInvoiceDomainService : ITripleInvoiceDomainService
{
    private readonly IRetailOrderRepository _retailOrderRepository;
    private readonly ITripleInvoiceRepository _tripleInvoiceRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICustomerInvoiceTitleNumberRepository _customerInvoiceTitleNumberRepository;
    private readonly ITripleInvoiceFactory _tripleInvoiceFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="retailOrderRepository">散镜订单仓储</param>
    /// <param name="tripleInvoiceRepository">三联发票仓储</param>
    /// <param name="storeRepository">门店仓储</param>
    /// <param name="customerInvoiceTitleNumberRepository">客户抬头发票号仓储</param>
    /// <param name="tripleInvoiceFactory">三联发票工厂</param>
    public TripleInvoiceDomainService(IRetailOrderRepository retailOrderRepository, ITripleInvoiceRepository tripleInvoiceRepository, IStoreRepository storeRepository, ICustomerInvoiceTitleNumberRepository customerInvoiceTitleNumberRepository, ITripleInvoiceFactory tripleInvoiceFactory)
    {
        _retailOrderRepository = retailOrderRepository;
        _tripleInvoiceRepository = tripleInvoiceRepository;
        _storeRepository = storeRepository;
        _customerInvoiceTitleNumberRepository = customerInvoiceTitleNumberRepository;
        _tripleInvoiceFactory = tripleInvoiceFactory;
    }

    /// <inheritdoc />
    public async Task<TripleInvoice?> CreateAsync(long retailOrderId, int? customerInvoiceTitlePadNumber, CancellationToken cancellationToken = default)
    {
        if (await _tripleInvoiceRepository.IsExistByRetailOrderIdAsync(retailOrderId, cancellationToken))
        {
            return null;
        }

        RetailOrder? retailOrder = await _retailOrderRepository.GetWithDetailsAsync(retailOrderId, cancellationToken);
        if (retailOrder is null)
        {
            throw new UserFriendlyException("散镜订单不存在!");
        }

        Store? store = await _storeRepository.FindAsync(retailOrder.StoreId, true, cancellationToken);
        if (store is null)
        {
            throw new UserFriendlyException("门店不存在!");
        }
        if (store.Number.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店" + store.Code + "编号为空!");
        }
        if (store.InvoiceTitle.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店" + store.Code + "发票抬头为空!");
        }

        RetailOrderItem? itemR = retailOrder.GetRetailOrderItems().FirstOrDefault(x => EyeTypeEnum.R == x.EyeType);
        if (itemR is null)
        {
            throw new UserFriendlyException("散镜订单项R不存在!");
        }

        RetailOrderItem? itemL = retailOrder.GetRetailOrderItems().FirstOrDefault(x => EyeTypeEnum.L == x.EyeType);
        if (itemL is null)
        {
            throw new UserFriendlyException("散镜订单项L不存在!");
        }

        CustomerInvoiceTitleNumber? customerInvoiceTitleNumber = await _customerInvoiceTitleNumberRepository.GetByInvoiceTitleAsync(store.InvoiceTitle!, cancellationToken);
        if (customerInvoiceTitleNumber is null)
        {
            customerInvoiceTitleNumber = new CustomerInvoiceTitleNumber(store.InvoiceTitle!, customerInvoiceTitlePadNumber ?? 0);
        }

        string customerInvoiceTitleNo = customerInvoiceTitleNumber.GenerateNumber();

        TripleInvoiceData data = new TripleInvoiceData(
            customerInvoiceTitleNo,
            retailOrder.Id,
            retailOrder.Number,
            retailOrder.CustomerOrderNumber,
            retailOrder.CustomerPONumber,
            retailOrder.StoreId,
            retailOrder.CompanyId,
            store.Code,
            store.Number!,
            store.InvoiceTitle!,
            store.Address,
            store.Telephone,
            itemR.SalesSpuId,
            itemR.SalesSpuCode,
            itemR.CustomerSpuId,
            itemR.CustomerSpuCode,
            itemR.SpuId,
            itemR.WarehouseId,
            itemR.DegreesType,
            itemR.Sph,
            itemR.Cyl,
            itemR.Add,
            itemR.Diameter,
            itemR.Axis,
            itemR.Pd,
            itemR.Ph,
            itemR.Base,
            itemR.CenterThickness,
            itemR.KnifeSide,
            itemR.Decenter,
            itemR.Prism,
            itemR.Qty,
            itemL.SalesSpuId,
            itemL.SalesSpuCode,
            itemL.CustomerSpuId,
            itemL.CustomerSpuCode,
            itemL.SpuId,
            itemL.WarehouseId,
            itemL.DegreesType,
            itemL.Sph,
            itemL.Cyl,
            itemL.Add,
            itemL.Diameter,
            itemL.Axis,
            itemL.Pd,
            itemL.Ph,
            itemL.Base,
            itemL.CenterThickness,
            itemL.KnifeSide,
            itemL.Decenter,
            itemL.Prism,
            itemL.Qty,
            retailOrder.Remark);

        return await _tripleInvoiceFactory.CreateAsync(data, cancellationToken);
    }
}
