using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性工厂, 负责创建属性
/// </summary>
[ScopedService]
public class AttributeFactory : IAttributeFactory
{
    private readonly IAttributeRepository _attributeRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="attributeRepository">属性仓储</param>
    public AttributeFactory(IAttributeRepository attributeRepository)
    {
        _attributeRepository = attributeRepository;
    }

    /// <inheritdoc />
    public async Task<Attribute> CreateAsync(ProductTypeEnum productType, AttributeScopeEnum scope, string code, string name, bool isKey, InputTypeEnum inputType, DataTypeEnum dataType, bool isRequired, bool isFilter, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _attributeRepository.IsExistByProductTypeAndScopeAndCodeExcludeIdAsync(productType, scope, code, null, cancellationToken))
        {
            throw new UserFriendlyException("属性代码" + code + "已存在!");
        }

        return new Attribute(productType, scope, code, name, isKey, inputType, dataType, isRequired, isFilter, sort, remark);
    }
}
