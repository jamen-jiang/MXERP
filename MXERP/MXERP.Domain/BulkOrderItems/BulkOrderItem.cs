using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项(按销售产品SPU聚合)
/// </summary>
public class BulkOrderItem : DeletionAuditedAggregateRoot
{
    /// <summary>
    /// 批量订单Id
    /// </summary>
    public long BulkOrderId { get; private set; }
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }
    /// <summary>
    /// 销售产品Code
    /// </summary>
    public string SalesSpuCode { get; private set; } = string.Empty;
    /// <summary>
    /// 客人产品Id(可空)
    /// </summary>
    public long? CustomerSpuId { get; private set; }
    /// <summary>
    /// 客人产品Code(可空)
    /// </summary>
    public string? CustomerSpuCode { get; private set; }
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }
    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }
    /// <summary>
    /// 包装信息(可空)
    /// </summary>
    public string? PacketInfo { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 抽仓数量
    /// </summary>
    public double DeductQty { get; private set; }
    /// <summary>
    /// 订购数量
    /// </summary>
    public double CustomizedQty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// 是否确认
    /// </summary>
    public bool IsConfirm { get; private set; }

    /// <summary>
    /// 销售SKU行列表
    /// </summary>
    private readonly List<BulkOrderItemSalesSku> _bulkOrderItemSalesSkus;

    /// <summary>
    /// 销售SKU行列表
    /// </summary>
    public IReadOnlyCollection<BulkOrderItemSalesSku> BulkOrderItemSalesSkus
    {
        get { return _bulkOrderItemSalesSkus.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected BulkOrderItem()
    {
        _bulkOrderItemSalesSkus = new List<BulkOrderItemSalesSku>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrderId">批量订单Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id(可空)</param>
    /// <param name="customerSpuCode">客人产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="packetInfo">包装信息(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="attachmentIds">附件Id集合(可空)</param>
    /// <param name="attachments">附件集合(可空)</param>
    internal BulkOrderItem(long bulkOrderId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments)
        : this()
    {
        BulkOrderId = bulkOrderId;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
        SpuId = spuId;
        WarehouseId = warehouseId;
        PacketInfo = packetInfo;
        Remark = remark;
        AddDomainEvent(new BulkOrderItemSavedEvent(this, attachmentIds, attachments));
    }

    /// <summary>
    /// 修改(BulkOrderId / SalesSpuId 不可改)
    /// </summary>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id(可空)</param>
    /// <param name="customerSpuCode">客人产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="packetInfo">包装信息(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="attachmentIds">附件Id集合(可空)</param>
    /// <param name="attachments">附件集合(可空)</param>
    internal void Modify(string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments)
    {
        if (CustomizedQty > 0 || DeductQty > 0)
        {
            throw new UserFriendlyException("批量订单项:" + Id + "已分配不允许修改!");
        }

        if (IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + Id + "已确认不允许修改!");
        }

        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
        SpuId = spuId;
        WarehouseId = warehouseId;
        PacketInfo = packetInfo;
        Remark = remark;
        AddDomainEvent(new BulkOrderItemSavedEvent(this, attachmentIds, attachments));
    }

    /// <summary>
    /// 欠数
    /// </summary>
    /// <returns>欠数</returns>
    public double GetOweQty()
    {
        return Qty - LeaveStockQty;
    }

    /// <summary>
    /// 未分配数量
    /// </summary>
    /// <returns>未分配数量</returns>
    public double GetNoAssignQty()
    {
        return Qty - DeductQty - CustomizedQty;
    }

    /// <summary>
    /// 是否已分配
    /// </summary>
    /// <returns>true 表示已分配</returns>
    public bool GetIsAssign()
    {
        return Math.Abs(DeductQty + CustomizedQty - Qty) < 1e-9;
    }

    /// <summary>
    /// 确认
    /// </summary>
    public void Confirm()
    {
        if (!GetIsAssign())
        {
            throw new UserFriendlyException("批量订单项:" + Id + "未分配不允许确认!");
        }

        if (IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + Id + "已确认!");
        }

        IsConfirm = true;
        AddDomainEvent(new BulkOrderItemConfirmedEvent(this));
    }

    /// <summary>
    /// 取消确认
    /// </summary>
    internal void UnConfirm()
    {
        if (!IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + Id + "已取消确认!");
        }

        IsConfirm = false;
        AddDomainEvent(new BulkOrderItemUnConfirmedEvent(this));
    }

    /// <summary>
    /// 更新出库数量
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    /// <param name="qty">数量</param>
    public void UpdateLeaveStockQty(long bulkOrderItemSalesSkuId, double qty)
    {
        BulkOrderItemSalesSku? salesSku = FindBulkOrderItemSalesSku(bulkOrderItemSalesSkuId);
        if (salesSku is null)
        {
            return;
        }

        salesSku.UpdateLeaveStockQty(qty);
        UpdateLeaveStockQtyTotal();
    }

    /// <summary>
    /// 更新抽仓数量
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    /// <param name="qty">数量</param>
    public void UpdateDeductQty(long bulkOrderItemSalesSkuId, double qty)
    {
        BulkOrderItemSalesSku? salesSku = FindBulkOrderItemSalesSku(bulkOrderItemSalesSkuId);
        if (salesSku is null)
        {
            return;
        }

        salesSku.UpdateDeductQty(qty);
        UpdateDeductQtyTotal();
    }

    /// <summary>
    /// 更新订购数量
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    /// <param name="qty">数量</param>
    public void UpdateCustomizedQty(long bulkOrderItemSalesSkuId, double qty)
    {
        BulkOrderItemSalesSku? salesSku = FindBulkOrderItemSalesSku(bulkOrderItemSalesSkuId);
        if (salesSku is null)
        {
            return;
        }

        salesSku.UpdateCustomizedQty(qty);
        UpdateCustomizedQtyTotal();
    }

    /// <summary>
    /// 新增SKU行
    /// </summary>
    /// <param name="salesSkuId">销售SkuId</param>
    /// <param name="skuAttributes">Sku关键属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    /// <param name="qty">数量</param>
    public void AddLine(long salesSkuId, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes, double qty)
    {
        if (qty <= 0)
        {
            return;
        }

        if (IsExistSalesSku(salesSkuId))
        {
            throw new UserFriendlyException("销售产品" + SalesSpuId + "销售Sku " + salesSkuId + " 已存在");
        }

        BulkOrderItemSalesSku salesSku = new BulkOrderItemSalesSku(SalesSpuId, salesSkuId, skuAttributes, otherAttributes, qty);
        _bulkOrderItemSalesSkus.Add(salesSku);
        UpdateQtyTotal();
    }

    /// <summary>
    /// 修改SKU行(数量小于等于0时删除)
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    /// <param name="qty">数量</param>
    public void ModifyLine(long bulkOrderItemSalesSkuId, SortedDictionary<string, object?>? otherAttributes, double qty)
    {
        BulkOrderItemSalesSku? salesSku = FindBulkOrderItemSalesSku(bulkOrderItemSalesSkuId);
        if (salesSku is null)
        {
            throw new UserFriendlyException("批量订单项" + Id + "销售Sku行 " + bulkOrderItemSalesSkuId + "不存在");
        }

        if (qty > 0)
        {
            salesSku.Modify(otherAttributes, qty);
        }
        else
        {
            salesSku.Delete();
        }

        UpdateQtyTotal();
    }

    /// <summary>
    /// 删除SKU行
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    public void DeleteLine(long bulkOrderItemSalesSkuId)
    {
        BulkOrderItemSalesSku? salesSku = FindBulkOrderItemSalesSku(bulkOrderItemSalesSkuId);
        if (salesSku is null)
        {
            return;
        }

        salesSku.Delete();
        UpdateQtyTotal();
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQtyTotal()
    {
        Qty = _bulkOrderItemSalesSkus.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总抽仓数量
    /// </summary>
    private void UpdateDeductQtyTotal()
    {
        DeductQty = _bulkOrderItemSalesSkus.Where(x => !x.IsDeleted).Sum(x => x.DeductQty);
    }

    /// <summary>
    /// 汇总订购数量
    /// </summary>
    private void UpdateCustomizedQtyTotal()
    {
        CustomizedQty = _bulkOrderItemSalesSkus.Where(x => !x.IsDeleted).Sum(x => x.CustomizedQty);
    }

    /// <summary>
    /// 汇总出库数量
    /// </summary>
    private void UpdateLeaveStockQtyTotal()
    {
        LeaveStockQty = _bulkOrderItemSalesSkus.Where(x => !x.IsDeleted).Sum(x => x.LeaveStockQty);
    }

    /// <summary>
    /// 判断销售SKU行是否存在(未删除)
    /// </summary>
    /// <param name="salesSkuId">销售SkuId</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistSalesSku(long salesSkuId)
    {
        return _bulkOrderItemSalesSkus.Any(x => x.SalesSkuId == salesSkuId && !x.IsDeleted);
    }

    /// <summary>
    /// 按主键查找销售SKU行
    /// </summary>
    /// <param name="bulkOrderItemSalesSkuId">销售SKU行Id</param>
    /// <returns>销售SKU行(可空), 不存在返回 null</returns>
    private BulkOrderItemSalesSku? FindBulkOrderItemSalesSku(long bulkOrderItemSalesSkuId)
    {
        return _bulkOrderItemSalesSkus.FirstOrDefault(x => x.Id == bulkOrderItemSalesSkuId);
    }
}
