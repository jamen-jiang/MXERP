using MXERP.Domain.Common.Enums;
using MXERP.Domain.BulkOrderItems;
using MXERP.Domain.Companies;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单领域服务, 负责跨聚合编排
/// </summary>
[ScopedService]
public class BulkOrderDomainService : IBulkOrderDomainService
{
    /// <summary>
    /// 公司仓储
    /// </summary>
    private readonly ICompanyRepository _companyRepository;
    /// <summary>
    /// 批量订单项仓储
    /// </summary>
    private readonly IBulkOrderItemRepository _bulkOrderItemRepository;
    /// <summary>
    /// 批量订单仓储
    /// </summary>
    private readonly IBulkOrderRepository _bulkOrderRepository;
    /// <summary>
    /// 流水号生成器
    /// </summary>
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="companyRepository">公司仓储</param>
    /// <param name="bulkOrderItemRepository">批量订单项仓储</param>
    /// <param name="bulkOrderRepository">批量订单仓储</param>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public BulkOrderDomainService(ICompanyRepository companyRepository, IBulkOrderItemRepository bulkOrderItemRepository, IBulkOrderRepository bulkOrderRepository, ISerialNumberGenerator serialNumberGenerator)
    {
        _companyRepository = companyRepository;
        _bulkOrderItemRepository = bulkOrderItemRepository;
        _bulkOrderRepository = bulkOrderRepository;
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<BulkOrder> CreateAsync(ProductTypeEnum productType, long storeId, long companyId, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default)
    {
        if (qty <= 0)
        {
            throw new UserFriendlyException("数量不能小于等于0!");
        }

        bool isExist = await _bulkOrderRepository.IsExistByCustomerOrderNumberAndCustomerPONumberAndProductTypeExcludeIdAsync(customerOrderNumber, customerPONumber, productType, null, cancellationToken);
        if (isExist)
        {
            string message = "客户单号" + customerOrderNumber;
            if (!string.IsNullOrWhiteSpace(customerPONumber))
            {
                message = message + "客户PO号" + customerPONumber;
            }

            throw new UserFriendlyException(message + "已存在!");
        }

        await ValidateAsync(productType, companyId, customerPONumber, null, cancellationToken);

        BulkOrder model = new BulkOrder(productType, storeId, companyId, customerOrderNumber, customerPONumber, orderRef, customerSerialNumber, transportWay, deliveryOn, qty, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }

    /// <inheritdoc />
    public async Task<BulkOrder> ModifyAsync(long id, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default)
    {
        if (qty <= 0)
        {
            throw new UserFriendlyException("数量不能小于等于0!");
        }

        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(id, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单不存在!");
        }

        await ModifyAsync(bulkOrder, customerOrderNumber, customerPONumber, orderRef, customerSerialNumber, transportWay, deliveryOn, qty, remark, cancellationToken);
        return bulkOrder;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(BulkOrder bulkOrder, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark, CancellationToken cancellationToken = default)
    {
        bool isExist = await _bulkOrderRepository.IsExistByCustomerOrderNumberAndCustomerPONumberAndProductTypeExcludeIdAsync(customerOrderNumber, customerPONumber, bulkOrder.ProductType, bulkOrder.Id, cancellationToken);
        if (isExist)
        {
            string message = "客户单号" + customerOrderNumber;
            if (!string.IsNullOrWhiteSpace(customerPONumber))
            {
                message = message + "客户PO号" + customerPONumber;
            }

            throw new UserFriendlyException(message + "已存在!");
        }

        await ValidateAsync(bulkOrder.ProductType, bulkOrder.CompanyId, customerPONumber, bulkOrder.Id, cancellationToken);

        bulkOrder.Modify(customerOrderNumber, customerPONumber, orderRef, customerSerialNumber, transportWay, deliveryOn, qty, remark);
    }

    /// <inheritdoc />
    public async Task<BulkOrder> CancelAsync(long id, long userId, string name, string? remark, CancellationToken cancellationToken = default)
    {
        BulkOrder? model = await _bulkOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("批量订单不存在!");
        }

        List<BulkOrderItem> bulkOrderItems = await _bulkOrderItemRepository.GetListByBulkOrderIdAsync(id, cancellationToken);
        bool hasConfirmed = bulkOrderItems.Any(x => x.IsConfirm);
        if (hasConfirmed)
        {
            throw new UserFriendlyException("批量订单已有确认的批量订单项,不允许取消!");
        }

        model.Cancel(userId, name, remark);
        return model;
    }

    /// <summary>
    /// 跨聚合校验: 公司存在 / SMD 公司下客户PO号 + 产品类型唯一
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="id">排除的主键Id(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ValidateAsync(ProductTypeEnum productType, long companyId, string? customerPONumber, long? id, CancellationToken cancellationToken = default)
    {
        Company? company = await _companyRepository.FindAsync(companyId, true, cancellationToken);
        if (company is null)
        {
            throw new UserFriendlyException("公司" + companyId + "不存在!");
        }

        if (!string.Equals(company.Code, "SMD", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(customerPONumber))
        {
            return;
        }

        bool isExist = await _bulkOrderRepository.IsExistByCustomerPONumberAndProductTypeExcludeIdAsync(customerPONumber, productType, id, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("客户PO号" + customerPONumber + "已存在!");
        }
    }
}
