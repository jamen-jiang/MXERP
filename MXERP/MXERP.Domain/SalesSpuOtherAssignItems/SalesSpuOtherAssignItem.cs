using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项
/// </summary>
public class SalesSpuOtherAssignItem : AuditedAggregateRoot
{
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }

    /// <summary>
    /// 焗色
    /// </summary>
    public string Coating { get; private set; } = string.Empty;

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected SalesSpuOtherAssignItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    internal SalesSpuOtherAssignItem(long salesSpuId, string coating, long spuId, long warehouseId)
    {
        SalesSpuId = salesSpuId;
        Coating = coating;
        SpuId = spuId;
        WarehouseId = warehouseId;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    internal void Modify(long salesSpuId, string coating, long spuId, long warehouseId)
    {
        SalesSpuId = salesSpuId;
        Coating = coating;
        SpuId = spuId;
        WarehouseId = warehouseId;
    }
}
