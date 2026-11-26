using MXERP.Domain.Common.Enums;
using MXERP.Domain.Common.Utils;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Stocks;

/// <summary>
/// 库存
/// </summary>
public class Stock : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;
    /// <summary>
    /// SpuId
    /// </summary>
    public long SpuId { get; private set; }
    /// <summary>
    /// SkuId
    /// </summary>
    public long SkuId { get; private set; }
    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }
    /// <summary>
    /// 库存
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 冻结库存
    /// </summary>
    public double FreezeQty { get; private set; }
    /// <summary>
    /// 上限库存
    /// </summary>
    public double UpperLimitQty { get; private set; }
    /// <summary>
    /// 下限库存
    /// </summary>
    public double LowerLimitQty { get; private set; }
    /// <summary>
    /// Sku属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected Stock()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="spuId">SpuId</param>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    public Stock(string productType, long spuId, long skuId, long warehouseId, SortedDictionary<string, object?>? skuAttributes)
    {
        ProductType = productType;
        SpuId = spuId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        if (skuAttributes is null)
        {
            SkuAttributes = null;
        }
        else
        {
            SkuAttributes = new SortedDictionary<string, object?>(skuAttributes);
        }
    }

    /// <summary>
    /// 实际数量（现存量 - 冻结库存）
    /// </summary>
    /// <returns>实际数量</returns>
    public double GetActualQty()
    {
        return Qty - FreezeQty;
    }

    /// <summary>
    /// 添加库存
    /// </summary>
    /// <param name="qty">数量</param>
    /// <param name="spuCode">产品代码（用于错误提示）</param>
    /// <param name="warehouseCode">仓库代码（用于错误提示）</param>
    public void AddQty(double qty, string spuCode, string warehouseCode)
    {
        if (qty == 0)
        {
            return;
        }

        Qty += qty;
        if (Qty < 0)
        {
            throw new UserFriendlyException("产品" + spuCode + "仓库" + warehouseCode + ":" + SkuDisplay() + "入库后库存不允许小于0!");
        }
    }

    /// <summary>
    /// 减去库存
    /// </summary>
    /// <param name="qty">数量</param>
    /// <param name="spuCode">产品代码（用于错误提示）</param>
    /// <param name="warehouseCode">仓库代码（用于错误提示）</param>
    public void SubtractQty(double qty, string spuCode, string warehouseCode)
    {
        if (qty == 0)
        {
            return;
        }

        Qty -= qty;
        if (Qty < 0)
        {
            throw new UserFriendlyException("产品" + spuCode + "仓库" + warehouseCode + ":" + SkuDisplay() + "扣库后库存不允许小于0!");
        }
    }

    /// <summary>
    /// 冻结库存
    /// </summary>
    /// <param name="qty">数量</param>
    /// <param name="spuCode">产品代码（用于错误提示）</param>
    /// <param name="warehouseCode">仓库代码（用于错误提示）</param>
    public void FreezeStock(double qty, string spuCode, string warehouseCode)
    {
        if (qty <= 0)
        {
            return;
        }

        if (GetActualQty() < qty)
        {
            throw new UserFriendlyException("产品" + spuCode + "仓库" + warehouseCode + ":" + SkuDisplay() + "库存不足冻结!");
        }

        FreezeQty += qty;
    }

    /// <summary>
    /// 重新冻结库存（不做可用量校验）
    /// </summary>
    /// <param name="qty">数量</param>
    public void FreezeStockAgain(double qty)
    {
        if (qty <= 0)
        {
            return;
        }

        FreezeQty += qty;
    }

    /// <summary>
    /// 解冻冻结库存
    /// </summary>
    /// <param name="qty">数量</param>
    /// <param name="spuCode">产品代码（用于错误提示）</param>
    /// <param name="warehouseCode">仓库代码（用于错误提示）</param>
    public void ThawStock(double qty, string spuCode, string warehouseCode)
    {
        if (qty <= 0)
        {
            return;
        }

        if (FreezeQty < qty)
        {
            throw new UserFriendlyException("产品" + spuCode + "仓库" + warehouseCode + ":" + SkuDisplay() + "冻结库存不足扣除!");
        }

        FreezeQty -= qty;
    }


    /// <summary>
    /// Sku属性展示串, 供错误提示使用
    /// </summary>
    /// <returns>Sku属性展示串</returns>
    private string SkuDisplay()
    {
        return SkuAttributeUtils.ToText(SkuAttributes);
    }
}
