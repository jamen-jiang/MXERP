using MXERP.Domain.Common.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单工厂
/// </summary>
[ScopedService]
public class PurchaseOrderFactory : IPurchaseOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public PurchaseOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<PurchaseOrder> CreateAsync(
        string productType,
        string relationType,
        long companyId,
        long? positionId,
        long supplierId,
        string? payMode,
        string? deliveryMode,
        DateTime? deliveryOn,
        string deliveryer,
        string deliveryerTelephone,
        string deliveryArea,
        string deliveryAddress,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        PurchaseOrder model = new PurchaseOrder(
            productType,
            relationType,
            companyId,
            positionId,
            supplierId,
            payMode,
            deliveryMode,
            deliveryOn,
            deliveryer,
            deliveryerTelephone,
            deliveryArea,
            deliveryAddress,
            remark);

        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);

        return model;
    }
}
