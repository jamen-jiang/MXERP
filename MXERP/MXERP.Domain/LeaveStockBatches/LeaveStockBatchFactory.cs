using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次工厂
/// </summary>
[ScopedService]
public class LeaveStockBatchFactory : ILeaveStockBatchFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public LeaveStockBatchFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<LeaveStockBatch> CreateAsync(string productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long storeId, long companyId, string relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark, CancellationToken cancellationToken = default)
    {
        LeaveStockBatch model = new LeaveStockBatch(productType, leaveStockPlanId, purchaseBatchId, purchaseBatchNumber, storeId, companyId, relationType, relationId, relationItemId, relationNumber, relationCustomerOrderNumber, relationCustomerPONumber, relationOrderRef, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }

    /// <inheritdoc />
    public async Task<LeaveStockBatch> CreateAsync(string productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long departmentId, long companyId, string relationType, long relationId, string relationNumber, string? remark, CancellationToken cancellationToken = default)
    {
        LeaveStockBatch model = new LeaveStockBatch(productType, leaveStockPlanId, purchaseBatchId, purchaseBatchNumber, departmentId, companyId, relationType, relationId, relationNumber, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
