using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单详情
/// </summary>
public class EnterStockOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected EnterStockOrderItem()
    {
        _enterStockOrderRelations = new List<EnterStockOrderRelation>();
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="originalId">原入库单项Id（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal EnterStockOrderItem(long? originalId, long spuId, long skuId, long warehouseId, double qty, decimal price, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
        : this()
    {
        OriginalId = originalId;
        SpuId = spuId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty;
        Price = price;
        SkuAttributes = skuAttributes;
        // 与 Java 保持一致：非关键属性不赋值
    }

    /// <summary>
    /// 入库单Id
    /// </summary>
    public long? EnterStockOrderId { get; private set; }

    /// <summary>
    /// 入出库单项Id（可空）
    /// </summary>
    public long? OriginalId { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; private set; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 冲销数量
    /// </summary>
    public double ReversalQty { get; private set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Sku 属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }

    /// <summary>
    /// 非关键属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 入库单项关联列表
    /// </summary>
    private readonly List<EnterStockOrderRelation> _enterStockOrderRelations;

    /// <summary>
    /// 入库单项关联列表
    /// </summary>
    public IReadOnlyCollection<EnterStockOrderRelation> EnterStockOrderRelations
    {
        get { return _enterStockOrderRelations.AsReadOnly(); }
    }

    /// <summary>
    /// 修改数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void Modify(double qty)
    {
        Qty = qty;
    }

    /// <summary>
    /// 添加数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddQty(double qty)
    {
        Qty += qty;
    }

    /// <summary>
    /// 更新冲销数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="qty">数量</param>
    internal void UpdateReversalQty(double qty)
    {
        ReversalQty += qty;
    }

    /// <summary>
    /// 添加入库单关联
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="qty">数量</param>
    internal void AddEnterStockOrderRelation(BusinessOrderTypeEnum relationType, long relationId, long relationItemId, string relationNumber, double qty)
    {
        _enterStockOrderRelations.Add(new EnterStockOrderRelation(relationType, relationId, relationItemId, relationNumber, qty));
    }
}
