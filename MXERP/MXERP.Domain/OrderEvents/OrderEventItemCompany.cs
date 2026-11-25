using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件项公司
/// </summary>
public class OrderEventItemCompany : DeletionAuditedEntity
{
    /// <summary>
    /// 订单事件项Id
    /// </summary>
    public long? OrderEventItemId { get; private set; }

    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected OrderEventItemCompany()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="companyId">公司Id</param>
    internal OrderEventItemCompany(long companyId)
    {
        CompanyId = companyId;
    }
}
