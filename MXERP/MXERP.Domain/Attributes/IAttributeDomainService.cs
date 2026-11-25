namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性领域服务接口
/// </summary>
public interface IAttributeDomainService
{
    /// <summary>
    /// 同步属性项列表(按Value匹配: 删除不在输入中的、更新已存在的、添加新的)
    /// </summary>
    /// <param name="attribute">属性聚合</param>
    /// <param name="attributeItems">目标属性项列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncAttributeItemsAsync(Attribute attribute, IReadOnlyCollection<AttributeItemData>? attributeItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步属性校验规则列表(按Expression匹配: 删除不在输入中的、更新已存在的、添加新的)
    /// </summary>
    /// <param name="attribute">属性聚合</param>
    /// <param name="attributeValidateRules">目标校验规则列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncAttributeValidateRulesAsync(Attribute attribute, IReadOnlyCollection<AttributeValidateRuleData>? attributeValidateRules, CancellationToken cancellationToken = default);
}
