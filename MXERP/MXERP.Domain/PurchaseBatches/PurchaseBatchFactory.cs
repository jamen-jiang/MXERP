using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次工厂
/// </summary>
[ScopedService]
public class PurchaseBatchFactory : IPurchaseBatchFactory
{
    /// <summary>
    /// 流水号生成器
    /// </summary>
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public PurchaseBatchFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<PurchaseBatch> CreateAsync(ProductTypeEnum productType, long? purchasePlanId, long storeId, long companyId, BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default)
    {
        PurchaseBatch model = new PurchaseBatch(productType, purchasePlanId, storeId, companyId, relationType, relationId, relationItemId, relationNumber, relationCustomerOrderNumber, relationCustomerPONumber, relationOrderRef, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }

    /// <inheritdoc />
    public async Task<PurchaseBatch> CreateAsync(ProductTypeEnum productType, long? purchasePlanId, long departmentId, long companyId, BusinessOrderTypeEnum relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default)
    {
        PurchaseBatch model = new PurchaseBatch(productType, purchasePlanId, departmentId, companyId, relationType, relationId, relationNumber, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
