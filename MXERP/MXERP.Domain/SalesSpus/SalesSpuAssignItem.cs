using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品分配项
/// </summary>
public class SalesSpuAssignItem : DeletionAuditedEntity
{
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long? SalesSpuId { get; private set; }

    /// <summary>
    /// 序列
    /// </summary>
    public int Sequence { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }

    /// <summary>
    /// 是否优先订购
    /// </summary>
    public bool IsPriorityCustomized { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected SalesSpuAssignItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="sequence">序列</param>
    /// <param name="isPriorityCustomized">是否优先订购</param>
    internal SalesSpuAssignItem(long spuId, long warehouseId, int sequence, bool? isPriorityCustomized)
    {
        SpuId = spuId;
        WarehouseId = warehouseId;
        Sequence = sequence;
        IsPriorityCustomized = isPriorityCustomized ?? false;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="sequence">序列</param>
    /// <param name="isPriorityCustomized">是否优先订购</param>
    internal void Modify(long spuId, long warehouseId, int sequence, bool? isPriorityCustomized)
    {
        SpuId = spuId;
        WarehouseId = warehouseId;
        Sequence = sequence;
        IsPriorityCustomized = isPriorityCustomized ?? false;
    }
}
