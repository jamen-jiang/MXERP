using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品路线(销售产品 x 产品 x 工艺路线)
/// </summary>
public class SalesSpuRouting : DeletionAuditedEntity
{
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long? SalesSpuId { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 路线Id
    /// </summary>
    public long RoutingId { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected SalesSpuRouting()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="routingId">路线Id</param>
    internal SalesSpuRouting(long spuId, long routingId)
    {
        if (spuId <= 0)
        {
            throw new UserFriendlyException("产品不能为空");
        }

        if (routingId <= 0)
        {
            throw new UserFriendlyException("路线不能为空");
        }

        SpuId = spuId;
        RoutingId = routingId;
    }

    /// <summary>
    /// 修改路线
    /// </summary>
    /// <param name="routingId">路线Id</param>
    internal void Modify(long routingId)
    {
        if (routingId <= 0)
        {
            throw new UserFriendlyException("路线不能为空");
        }

        RoutingId = routingId;
    }
}
