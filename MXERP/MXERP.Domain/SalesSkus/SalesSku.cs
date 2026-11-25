using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Common.Utils;

namespace MXERP.Domain.SalesSkus;

/// <summary>
/// 销售SKU
/// </summary>
public class SalesSku : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;

    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }

    /// <summary>
    /// 属性标识码
    /// </summary>
    public string AttributesSignature { get; private set; } = string.Empty;

    /// <summary>
    /// Opc
    /// </summary>
    public string? Opc { get; private set; }

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
    protected SalesSku()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="keyAttributes">关键属性值</param>
    /// <param name="otherAttributes">非关键属性值</param>
    internal SalesSku(string productType, long companyId, long salesSpuId, string? opc, SortedDictionary<string, object?>? keyAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        ValidateProductType(productType);
        ProductType = productType;
        CompanyId = companyId;
        SalesSpuId = salesSpuId;
        Opc = opc;
        ChangeKeyAttributes(keyAttributes);
        ChangeOtherAttributes(otherAttributes);
        AttributesSignature = CalculateSignature();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="opc">Opc</param>
    /// <param name="otherAttributes">非关键属性值</param>
    internal void Modify(string? opc, SortedDictionary<string, object?>? otherAttributes)
    {
        Opc = opc;
        ChangeOtherAttributes(otherAttributes);
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
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="keyAttributes">关键属性值</param>
    /// <returns>MD5特征值</returns>
    public static string CalculateSignature(long salesSpuId, SortedDictionary<string, object?>? keyAttributes)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder(salesSpuId.ToString());
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
        return CalculateSignature(SalesSpuId, KeyAttributes);
    }

    /// <summary>
    /// 校验产品类型
    /// </summary>
    /// <param name="productType">产品类型</param>
    private static void ValidateProductType(string productType)
    {
        if (string.IsNullOrWhiteSpace(productType))
        {
            throw new UserFriendlyException("产品类型不能为空");
        }

        if (!ProductTypeEnum.IsValid(productType))
        {
            throw new UserFriendlyException("产品类型" + productType + "有误!");
        }
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
