namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件项数据
/// </summary>
/// <param name="Id">订单事件项Id</param>
/// <param name="To">指定收件人</param>
/// <param name="Cc">指定抄送人</param>
/// <param name="Remark">备注</param>
/// <param name="CompanyIds">公司Id列表</param>
public sealed record OrderEventItemData(
    long? Id,
    string? To,
    string? Cc,
    string? Remark,
    IReadOnlyCollection<long> CompanyIds);
