using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典领域服务
/// </summary>
[ScopedService]
public class DictionaryDomainService : IDictionaryDomainService
{
    private readonly DictionaryFactory _factory;
    private readonly IDictionaryRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">字典工厂</param>
    /// <param name="repository">字典仓储</param>
    public DictionaryDomainService(DictionaryFactory factory, IDictionaryRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Dictionary> ModifyAsync(long id, string category, string name, string? remark, CancellationToken cancellationToken = default)
    {
        Dictionary? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("字典不存在!");
        }

        await ModifyAsync(model, category, name, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public Task SyncDictionaryItemsAsync(Dictionary dictionary, IReadOnlyList<DictionaryItemData>? dictionaryItems, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<DictionaryItemData> inputs = dictionaryItems ?? Array.Empty<DictionaryItemData>();

        List<DictionaryItem> toRemoves = dictionary.GetDictionaryItems()
            .Where(a => !inputs.Any(i => i.Value == a.Value))
            .ToList();
        foreach (DictionaryItem item in toRemoves)
        {
            dictionary.DeleteItem(item.Id);
        }

        Dictionary<string, long> existingDict = new();
        foreach (DictionaryItem item in dictionary.GetDictionaryItems())
        {
            existingDict.TryAdd(item.Value, item.Id);
        }

        foreach (DictionaryItemData item in inputs)
        {
            if (existingDict.TryGetValue(item.Value, out long id))
            {
                dictionary.ModifyItem(id, item.Value, item.Label, item.Sort, item.Remark);
            }
            else
            {
                dictionary.AddItem(item.Value, item.Label, item.Sort, item.Remark);
            }
        }

        return Task.CompletedTask;
    }

    private async Task ModifyAsync(Dictionary model, string category, string name, string? remark, CancellationToken cancellationToken)
    {
        if (await _repository.IsExistByCategoryExcludeIdAsync(category, model.Id, cancellationToken))
        {
            throw new UserFriendlyException($"字典类型{category}已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, model.Id, cancellationToken))
        {
            throw new UserFriendlyException($"字典类型名称{name}已存在!");
        }

        model.Modify(category, name, remark);
    }
}
