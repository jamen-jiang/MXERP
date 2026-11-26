using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Common.Utils;

namespace MXERP.Domain.Skus;

/// <summary>
/// SKU
/// </summary>
public class Sku : AuditedAggregateRoot
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
    /// 属性标识码
    /// </summary>
    public string AttributesSignature { get; private set; } = string.Empty;

    /// <summary>
    /// OPC
    /// </summary>
    public string? Opc { get; private set; }

    /// <summary>
    /// 采购价格
    /// </summary>
    public decimal? PurchasePrice { get; private set; }

    /// <summary>
    /// 币种Id
    /// </summary>
    public long? CurrencyId { get; private set; }

    /// <summary>
    /// 单位Id
    /// </summary>
    public long? UnitId { get; private set; }

    /// <summary>
    /// 税率Id
    /// </summary>
    public long? TaxRateId { get; private set; }

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool Disabled { get; private set; }

    /// <summary>
    /// 起订量
    /// </summary>
    public decimal? Moq { get; private set; }

    /// <summary>
    /// 关键属性值(参与唯一性判断)
    /// </summary>
    public SortedDictionary<string, object?>? KeyAttributes { get; private set; }

    /// <summary>
    /// 非关键属性值(不参与唯一性判断)
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected Sku()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="spuId">SpuId</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="keyAttributes">关键属性值</param>
    /// <param name="otherAttributes">非关键属性值</param>
    internal Sku(
        string productType,
        long spuId,
        string? opc,
        decimal? purchasePrice,
        long? currencyId,
        long? unitId,
        long? taxRateId,
        decimal? moq,
        SortedDictionary<string, object?>? keyAttributes,
        SortedDictionary<string, object?>? otherAttributes)
    {
        ProductType = productType;
        SpuId = spuId;
        Opc = opc;
        PurchasePrice = purchasePrice;
        CurrencyId = currencyId;
        UnitId = unitId;
        TaxRateId = taxRateId;
        Moq = moq;
        ChangeKeyAttributes(keyAttributes);
        ChangeOtherAttributes(otherAttributes);
        AttributesSignature = CalculateSignature();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="otherAttributes">非关键属性值</param>
    public void Modify(
        string? opc,
        decimal? purchasePrice,
        long? currencyId,
        long? unitId,
        long? taxRateId,
        decimal? moq,
        SortedDictionary<string, object?>? otherAttributes)
    {
        Opc = opc;
        PurchasePrice = purchasePrice;
        CurrencyId = currencyId;
        UnitId = unitId;
        TaxRateId = taxRateId;
        Moq = moq;
        ChangeOtherAttributes(otherAttributes);
    }

    /// <summary>
    /// 启用
    /// </summary>
    public void Enable()
    {
        Disabled = false;
    }

    /// <summary>
    /// 禁用
    /// </summary>
    public void Disable()
    {
        Disabled = true;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new SkuDeletedEvent(this));
    }

    /// <summary>
    /// 是否有关键属性
    /// </summary>
    /// <returns>true 表示有关键属性</returns>
    public bool HasKeyAttributes()
    {
        return !string.IsNullOrWhiteSpace(AttributesSignature) && Id > 0;
    }

    /// <summary>
    /// 合并所有属性
    /// </summary>
    /// <returns>合并后的属性</returns>
    public SortedDictionary<string, object?> Attributes()
    {
        SortedDictionary<string, object?> all = new SortedDictionary<string, object?>();
        if (KeyAttributes != null)
        {
            foreach (KeyValuePair<string, object?> pair in KeyAttributes)
            {
                all[pair.Key] = pair.Value;
            }
        }

        if (OtherAttributes != null)
        {
            foreach (KeyValuePair<string, object?> pair in OtherAttributes)
            {
                all[pair.Key] = pair.Value;
            }
        }

        return all;
    }

    /// <summary>
    /// 静态工具方法: 在不创建实例的情况下计算特征值
    /// </summary>
    /// <param name="spuId">SpuId</param>
    /// <param name="keyAttributes">关键属性值</param>
    /// <returns>MD5特征值</returns>
    public static string CalculateSignature(long spuId, SortedDictionary<string, object?>? keyAttributes)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder(spuId.ToString());
        if (keyAttributes != null && keyAttributes.Count > 0)
        {
            SortedDictionary<string, object?> ordered = new SortedDictionary<string, object?>(keyAttributes);
            string joined = string.Join("|", ordered.Values.Select(v => v == null ? "null" : v.ToString()));
            builder.Append('|').Append(joined);
        }

        return HashUtils.ComputeMd5(builder.ToString());
    }

    /// <summary>
    /// 计算特征值
    /// </summary>
    /// <returns>MD5特征值</returns>
    private string CalculateSignature()
    {
        return CalculateSignature(SpuId, KeyAttributes);
    }


    /// <summary>
    /// 变更关键属性值(自动包装为有序字典)
    /// </summary>
    /// <param name="attributes">关键属性值</param>
    private void ChangeKeyAttributes(SortedDictionary<string, object?>? attributes)
    {
        KeyAttributes = attributes == null ? null : new SortedDictionary<string, object?>(attributes);
    }

    /// <summary>
    /// 变更非关键属性值(自动包装为有序字典)
    /// </summary>
    /// <param name="attributes">非关键属性值</param>
    private void ChangeOtherAttributes(SortedDictionary<string, object?>? attributes)
    {
        OtherAttributes = attributes == null ? null : new SortedDictionary<string, object?>(attributes);
    }
}
