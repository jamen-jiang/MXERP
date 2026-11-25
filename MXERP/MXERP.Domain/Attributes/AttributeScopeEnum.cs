using System.ComponentModel;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性范围
/// </summary>
public enum AttributeScopeEnum
{
    /// <summary>
    /// 产品
    /// </summary>
    [Description("产品")]
    Spu,

    /// <summary>
    /// Sku
    /// </summary>
    [Description("Sku")]
    Sku,

    /// <summary>
    /// 销售产品
    /// </summary>
    [Description("销售产品")]
    SalesSpu,

    /// <summary>
    /// 销售Sku
    /// </summary>
    [Description("销售Sku")]
    SalesSku,

    /// <summary>
    /// 公用
    /// </summary>
    [Description("公用")]
    Common
}
