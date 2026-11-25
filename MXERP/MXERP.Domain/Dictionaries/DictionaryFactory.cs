using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Dictionaries;

/// <summary>
/// 字典工厂: 负责创建前的类型/名称唯一性校验
/// </summary>
[ScopedService]
public class DictionaryFactory : IDictionaryFactory
{
    private readonly IDictionaryRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">字典仓储</param>
    public DictionaryFactory(IDictionaryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Dictionary> CreateAsync(string category, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCategoryExcludeIdAsync(category, null, cancellationToken))
        {
            throw new UserFriendlyException($"字典类型{category}已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException($"字典类型名称{name}已存在!");
        }

        return new Dictionary(category, name, remark);
    }
}
