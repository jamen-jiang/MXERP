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
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 度数类型(+/- 正散 / -/- 负散)
    /// </summary>
    public string DegreesType { get; private set; } = string.Empty;

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
    internal EndProductCodeConfig(string category, long spuId, string degreesType, int? sort)
    {
        Category = ValidateCategory(category);
        SpuId = spuId;
        DegreesType = ValidateDegreesType(degreesType);
        Sort = sort;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="category">分类(Total / Inactive)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="degreesType">度数类型</param>
    /// <param name="sort">排序(可空)</param>
    internal void Modify(string category, long spuId, string degreesType, int? sort)
    {
        Category = ValidateCategory(category);
        SpuId = spuId;
        DegreesType = ValidateDegreesType(degreesType);
        Sort = sort;
    }

    /// <summary>
    /// 校验分类
    /// </summary>
    /// <param name="category">分类</param>
    /// <returns>校验并去空格后的分类</returns>
    private static string ValidateCategory(string? category)
    {
        string? trimmed = category?.Trim();
        if (trimmed is null || !EndProductCodeCategoryEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"分类{category}有误!");
        }

        return trimmed;
    }

    /// <summary>
    /// 校验度数类型
    /// </summary>
    /// <param name="degreesType">度数类型</param>
    /// <returns>校验并去空格后的度数类型</returns>
    private static string ValidateDegreesType(string? degreesType)
    {
        string? trimmed = degreesType?.Trim();
        if (trimmed is null || !DegreesTypeEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"度数类型{degreesType}有误!");
        }

        return trimmed;
    }
}
