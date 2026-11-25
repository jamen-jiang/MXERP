using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码配置
/// </summary>
public class EndProductCodeConfig : AuditedAggregateRoot
{
    /// <summary>
    /// 分类(Total / Inactive)
    /// </summary>
    public EndProductCodeCategoryEnum Category { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 度数类型(+/- 正散 / -/- 负散)
    /// </summary>
    public DegreesTypeEnum DegreesType { get; private set; }

    /// <summary>
    /// 排序(可空)
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected EndProductCodeConfig()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    internal EndProductCodeConfig(EndProductCodeCategoryEnum category, long spuId, DegreesTypeEnum degreesType, int? sort)
    {
        Category = category;
        SpuId = spuId;
        DegreesType = degreesType;
        Sort = sort;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    internal void Modify(EndProductCodeCategoryEnum category, long spuId, DegreesTypeEnum degreesType, int? sort)
    {
        Category = category;
        SpuId = spuId;
        DegreesType = degreesType;
        Sort = sort;
    }




}
