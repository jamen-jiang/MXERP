using Snow.DependencyInjection;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性领域服务, 负责属性子实体同步
/// </summary>
[ScopedService]
public class AttributeDomainService : IAttributeDomainService
{
    /// <inheritdoc />
    public Task SyncAttributeItemsAsync(Attribute attribute, IReadOnlyCollection<AttributeItemData>? attributeItems, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AttributeItemData> target = attributeItems ?? Array.Empty<AttributeItemData>();

        List<AttributeItem> toRemoves = attribute.AttributeItems
            .Where(i => !target.Any(d => d.Value == i.Value))
            .ToList();
        foreach (AttributeItem item in toRemoves)
        {
            attribute.DeleteItem(item.Id);
        }

        Dictionary<string, long> existingDict = attribute.AttributeItems
            .Where(i => !i.IsDeleted)
            .GroupBy(i => i.Value)
            .ToDictionary(g => g.Key, g => g.First().Id);

        foreach (AttributeItemData item in target)
        {
            long? id = existingDict.TryGetValue(item.Value, out long existingId) ? existingId : null;
            if (id != null)
            {
                attribute.ModifyItem(id.Value, item.Name, item.IsDefault, item.Sort);
            }
            else
            {
                attribute.AddItem(item.Value, item.Name, item.IsDefault, item.Sort);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SyncAttributeValidateRulesAsync(Attribute attribute, IReadOnlyCollection<AttributeValidateRuleData>? attributeValidateRules, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AttributeValidateRuleData> target = attributeValidateRules ?? Array.Empty<AttributeValidateRuleData>();

        List<AttributeValidateRule> toRemoves = attribute.AttributeValidateRules
            .Where(r => !target.Any(d => d.Expression == r.Expression))
            .ToList();
        foreach (AttributeValidateRule rule in toRemoves)
        {
            attribute.DeleteValidateRule(rule.Id);
        }

        Dictionary<string, long> existingDict = attribute.AttributeValidateRules
            .Where(r => !r.IsDeleted)
            .GroupBy(r => r.Expression)
            .ToDictionary(g => g.Key, g => g.First().Id);

        foreach (AttributeValidateRuleData item in target)
        {
            long? id = existingDict.TryGetValue(item.Expression, out long existingId) ? existingId : null;
            if (id != null)
            {
                attribute.ModifyValidateRule(id.Value, item.Expression, item.ErrorMessage, item.Sort);
            }
            else
            {
                attribute.AddValidateRule(item.Expression, item.ErrorMessage, item.Sort);
            }
        }

        return Task.CompletedTask;
    }
}
