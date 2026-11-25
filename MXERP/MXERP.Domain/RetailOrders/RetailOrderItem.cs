using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单项(对应 Java RetailOrderItem)
/// </summary>
public class RetailOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 散镜订单Id
    /// </summary>
    public long? RetailOrderId { get; private set; }
    /// <summary>
    /// 眼睛类型(R或L)
    /// </summary>
    public string EyeType { get; private set; } = string.Empty;
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
    /// 度数类型
    /// </summary>
    public string DegreesType { get; private set; } = string.Empty;
    /// <summary>
    /// SPH
    /// </summary>
    public int Sph { get; private set; }
    /// <summary>
    /// CYL
    /// </summary>
    public int Cyl { get; private set; }
    /// <summary>
    /// ADD(可空)
    /// </summary>
    public int? Add { get; private set; }
    /// <summary>
    /// 直径(可空)
    /// </summary>
    public int? Diameter { get; private set; }
    /// <summary>
    /// 轴位(可空)
    /// </summary>
    public int? Axis { get; private set; }
    /// <summary>
    /// 瞳距(可空)
    /// </summary>
    public double? Pd { get; private set; }
    /// <summary>
    /// 镜高(可空)
    /// </summary>
    public double? Ph { get; private set; }
    /// <summary>
    /// 面弯(可空)
    /// </summary>
    public string? Base { get; private set; }
    /// <summary>
    /// 中心厚度(可空)
    /// </summary>
    public double? CenterThickness { get; private set; }
    /// <summary>
    /// 刀边(可空)
    /// </summary>
    public bool? KnifeSide { get; private set; }
    /// <summary>
    /// 移心(可空)
    /// </summary>
    public string? Decenter { get; private set; }
    /// <summary>
    /// 棱镜(可空)
    /// </summary>
    public string? Prism { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected RetailOrderItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="eyeType">眼别</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id(可空)</param>
    /// <param name="customerSpuCode">客人产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="sph">SPH</param>
    /// <param name="cyl">CYL</param>
    /// <param name="add">ADD(可空)</param>
    /// <param name="diameter">直径(可空)</param>
    /// <param name="axis">轴位(可空)</param>
    /// <param name="pd">瞳距(可空)</param>
    /// <param name="ph">镜高(可空)</param>
    /// <param name="base">面弯(可空)</param>
    /// <param name="centerThickness">中心厚度(可空)</param>
    /// <param name="knifeSide">刀边(可空)</param>
    /// <param name="decenter">移心(可空)</param>
    /// <param name="prism">棱镜(可空)</param>
    /// <param name="qty">数量</param>
    internal RetailOrderItem(string eyeType, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, int sph, int cyl, int? add, int? diameter, int? axis, double? pd, double? ph, string? baseCurve, double? centerThickness, bool? knifeSide, string? decenter, string? prism, double qty)
    {
        if (qty > 1)
        {
            throw new UserFriendlyException("一个眼只允许最多下1片!");
        }

        EyeType = eyeType;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
        SpuId = spuId;
        WarehouseId = warehouseId;
        Sph = sph;
        Cyl = cyl;
        if (sph > 0)
        {
            DegreesType = DegreesTypeEnum.ZhengSan;
        }
        else
        {
            DegreesType = DegreesTypeEnum.FuSan;
        }
        Add = add;
        Diameter = diameter;
        Axis = axis;
        Pd = pd;
        Ph = ph;
        Base = baseCurve;
        CenterThickness = centerThickness;
        KnifeSide = knifeSide;
        Decenter = decenter;
        Prism = prism;
        Qty = qty;
    }

    /// <summary>
    /// 修改订单项
    /// </summary>
    internal void Modify(string eyeType, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, int sph, int cyl, int? add, int? diameter, int? axis, double? pd, double? ph, string? baseCurve, double? centerThickness, bool? knifeSide, string? decenter, string? prism, double qty)
    {
        EyeType = eyeType;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
        SpuId = spuId;
        WarehouseId = warehouseId;
        Sph = sph;
        Cyl = cyl;
        if (sph > 0)
        {
            DegreesType = DegreesTypeEnum.ZhengSan;
        }
        else
        {
            DegreesType = DegreesTypeEnum.FuSan;
        }
        Add = add;
        Diameter = diameter;
        Axis = axis;
        Pd = pd;
        Ph = ph;
        Base = baseCurve;
        CenterThickness = centerThickness;
        KnifeSide = knifeSide;
        Decenter = decenter;
        Prism = prism;
        Qty = qty;
    }

    /// <summary>
    /// 更新出库数量(正数=添加, 负数=减去)
    /// </summary>
    /// <param name="leaveStockQty">出库数量</param>
    internal void UpdateLeaveStockQty(double leaveStockQty)
    {
        LeaveStockQty += leaveStockQty;
        if (LeaveStockQty > Qty)
        {
            throw new UserFriendlyException("出库数量不允许大于散镜订单数量!");
        }
        if (LeaveStockQty < 0)
        {
            throw new UserFriendlyException("散镜订单出库数量不允许小于0!");
        }
    }
}
