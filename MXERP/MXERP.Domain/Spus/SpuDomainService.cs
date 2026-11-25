using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Attachments;
using MXERP.Domain.Attributes;

namespace MXERP.Domain.Spus;

/// <summary>
/// 产品领域服务
/// </summary>
[ScopedService]
public class SpuDomainService : ISpuDomainService
{
    private readonly ISpuRepository _spuRepository;
    private readonly IAttributeRepository _attributeRepository;
    private readonly IAttachmentRepository _attachmentRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="attributeRepository">属性仓储</param>
    /// <param name="attachmentRepository">附件仓储</param>
    public SpuDomainService(ISpuRepository spuRepository, IAttributeRepository attributeRepository, IAttachmentRepository attachmentRepository)
    {
        _spuRepository = spuRepository;
        _attributeRepository = attributeRepository;
        _attachmentRepository = attachmentRepository;
    }

    /// <inheritdoc />
    public async Task<Spu> CreateAsync(ProductTypeEnum productType, SpuGroupEnum? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default)
    {
        if (await _spuRepository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException("产品代码" + code + "已存在!");
        }

        await ValidateAttributesAsync(productType, attributes, cancellationToken);
        return new Spu(productType, group, supplierId, brandId, code, name, description, purchasePrice, currencyId, unitId, taxRateId, moq, remark, attributes, companyIds, attachmentIds);
    }

    /// <inheritdoc />
    public async Task<Spu> ImportCreateAsync(ProductTypeEnum productType, SpuGroupEnum? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, CancellationToken cancellationToken = default)
    {
        Spu? model = await _spuRepository.GetByProductTypeAndCodeAsync(productType, code, cancellationToken);
        if (model == null)
        {
            model = await CreateAsync(productType, group, supplierId, brandId, code, name, description, purchasePrice, currencyId, unitId, taxRateId, moq, remark, attributes, companyIds, null, cancellationToken);
        }
        else
        {
            List<Attachment> attachments = await _attachmentRepository.GetByOwnerTypeAndOwnerIdAsync(AttachmentOwnerTypeEnum.Spu, model.Id, cancellationToken);
            await ModifyAsync(model, group, supplierId, brandId, code, name, description, purchasePrice, currencyId, unitId, taxRateId, moq, remark, attributes, companyIds, attachments.Select(x => x.Id).ToList(), cancellationToken);
        }

        return model;
    }

    /// <inheritdoc />
    public async Task<Spu> ModifyAsync(long id, SpuGroupEnum? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default)
    {
        Spu? model = await _spuRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("产品不存在!");
        }

        await ModifyAsync(model, group, supplierId, brandId, code, name, description, purchasePrice, currencyId, unitId, taxRateId, moq, remark, attributes, companyIds, attachmentIds, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Spu spu, SpuGroupEnum? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default)
    {
        if (await _spuRepository.IsExistByCodeExcludeIdAsync(code, spu.Id, cancellationToken))
        {
            throw new UserFriendlyException("产品代码" + code + "已存在!");
        }

        await ValidateAttributesAsync(spu.ProductType, attributes, cancellationToken);
        spu.Modify(group, supplierId, brandId, code, name, description, purchasePrice, currencyId, unitId, taxRateId, moq, remark, attributes, companyIds, attachmentIds);
    }

    /// <summary>
    /// 校验属性存在性
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task ValidateAttributesAsync(ProductTypeEnum productType, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        if (attributes == null)
        {
            return;
        }

        foreach (string attributeCode in attributes.Keys)
        {
            if (!await _attributeRepository.IsExistByProductTypeAndScopeAndCodeExcludeIdAsync(productType, AttributeScopeEnum.Spu, attributeCode, null, cancellationToken))
            {
                throw new UserFriendlyException("属性" + attributeCode + "不存在!");
            }
        }
    }
}