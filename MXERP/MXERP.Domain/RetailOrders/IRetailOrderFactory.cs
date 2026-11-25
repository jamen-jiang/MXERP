namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单工厂接口
/// </summary>
public interface IRetailOrderFactory
{
    /// <summary>
    /// 创建散镜订单
    /// </summary>
    Task<RetailOrder> CreateAsync(long storeId, long companyId, string? outArea, string? sequenceNumber, string? hkNumber, string customerOrderNumber, string? customerPONumber, string? documentaryBarCode, int outGood, string? outGoodTime, string? material, string? coating, string? tintingColor, string? mirror, string? tintingCode, string? tintingName, bool? uv, bool? hard, bool? polish, string? auro, string? edge, string? superHole, string? special, string? specialCode, string? zuanKong, string? frame, string? remark, CancellationToken cancellationToken = default);
}
