using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Attributes;
using AttributeEntity = MXERP.Domain.Attributes.Attribute;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Spus;

namespace MXERP.Domain.Skus;

/// <summary>
/// SKU领域服务
/// </summary>
[ScopedService]
public class SkuDomainService : ISkuDomainService
{
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IAttributeRepository _attributeRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuRepository">SKU仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="attributeRepository">属性仓储</param>
    public SkuDomainService(ISkuRepository skuRepository, ISpuRepository spuRepository, IAttributeRepository attributeRepository)
    {
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _attributeRepository = attributeRepository;
    }

    /// <inheritdoc />
    public async Task<Sku> CreateAsync(string productType, long spuId, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        SplitAttributes split = await SplitAttributesAsync(productType, attributes, cancellationToken);
        return await BuildSkuAsync(productType, spuId, opc, purchasePrice, currencyId, unitId, taxRateId, moq, split.Key, split.Other, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Sku> ImportGenerateAsync(string productType, long spuId, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        SplitAttributes split = await SplitAttributesAsync(productType, attributes, cancellationToken);
        string signature = Sku.CalculateSignature(spuId, split.Key);
        Sku? model = await _skuRepository.GetByAttributesSignatureAsync(signature, cancellationToken);
        if (model == null)
        {
            model = await BuildSkuAsync(productType, spuId, opc, purchasePrice, currencyId, unitId, taxRateId, moq, split.Key, split.Other, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, opc, purchasePrice, currencyId, unitId, taxRateId, moq, attributes, cancellationToken);
        }

        return model;
    }

    /// <inheritdoc />
    public async Task<Sku> ModifyAsync(long id, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        Sku? model = await _skuRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("产品Sku不存在!");
        }

        await ModifyAsync(model, opc, purchasePrice, currencyId, unitId, taxRateId, moq, attributes, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task<List<Sku>> CreateFromSpuAsync(long spuId, CancellationToken cancellationToken = default)
    {
        Spu? spu = await _spuRepository.FindAsync(spuId, true, cancellationToken);
        if (spu == null)
        {
            throw new UserFriendlyException("产品" + spuId + "不存在!");
        }

        List<Sku> existing = await _skuRepository.GetListBySpuIdAsync(spu.Id, cancellationToken);
        List<Sku> toSave = new List<Sku>();

        if (ProductTypeEnum.Accessory == spu.ProductType)
        {
            await CreateForAccessoryAsync(spu, existing, toSave, cancellationToken);
        }
        else if (ProductTypeEnum.EndProduct == spu.ProductType)
        {
            await CreateForEndProductAsync(spu, existing, toSave, cancellationToken);
        }

        return toSave;
    }

    /// <summary>
    /// 构建SKU(含产品存在性、OPC唯一性、属性组合唯一性校验)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="spuId">SpuId</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="keyAttributes">关键属性(已拆分)</param>
    /// <param name="otherAttributes">非关键属性(已拆分)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU</returns>
    private async Task<Sku> BuildSkuAsync(string productType, long spuId, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? keyAttributes, SortedDictionary<string, object?>? otherAttributes, CancellationToken cancellationToken)
    {
        Spu? spu = await _spuRepository.FindAsync(spuId, true, cancellationToken);
        if (spu == null)
        {
            throw new UserFriendlyException("产品" + spuId + "不存在!");
        }

        if (!string.IsNullOrWhiteSpace(opc) && await _skuRepository.IsExistBySpuIdAndOpcExcludeIdAsync(spuId, opc, null, cancellationToken))
        {
            throw new UserFriendlyException("OPC:" + opc + "已存在!");
        }

        Sku model = new Sku(productType, spuId, opc, purchasePrice, currencyId, unitId, taxRateId, moq, keyAttributes, otherAttributes);

        if (await _skuRepository.IsExistByAttributesSignatureAsync(model.AttributesSignature, cancellationToken))
        {
            throw new UserFriendlyException("相同属性组合的Sku已存在!");
        }

        return model;
    }

    /// <summary>
    /// 修改(使用已有聚合)
    /// </summary>
    /// <param name="model">SKU聚合</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task ModifyAsync(Sku model, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(opc) && await _skuRepository.IsExistBySpuIdAndOpcExcludeIdAsync(model.SpuId, opc, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("OPC:" + opc + "已存在!");
        }

        SortedDictionary<string, object?> otherAttributes = await FilterOtherAttributesAsync(model.ProductType, attributes, cancellationToken);
        model.Modify(opc, purchasePrice, currencyId, unitId, taxRateId, moq, otherAttributes);
    }

    /// <summary>
    /// 拆分属性为关键/非关键
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>拆分结果</returns>
    private async Task<SplitAttributes> SplitAttributesAsync(string productType, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        SortedDictionary<string, object?> key = new SortedDictionary<string, object?>();
        SortedDictionary<string, object?> other = new SortedDictionary<string, object?>();
        if (attributes != null)
        {
            foreach (KeyValuePair<string, object?> entry in attributes)
            {
                AttributeEntity attr = await GetAttributeAsync(productType, entry.Key, cancellationToken);
                if (attr.IsKey)
                {
                    key[entry.Key] = entry.Value;
                }
                else
                {
                    other[entry.Key] = entry.Value;
                }
            }
        }

        return new SplitAttributes(key, other);
    }

    /// <summary>
    /// 只保留非关键属性(关键属性改了会变签名)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>非关键属性字典</returns>
    private async Task<SortedDictionary<string, object?>> FilterOtherAttributesAsync(string productType, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        SortedDictionary<string, object?> other = new SortedDictionary<string, object?>();
        if (attributes != null)
        {
            foreach (KeyValuePair<string, object?> entry in attributes)
            {
                AttributeEntity attr = await GetAttributeAsync(productType, entry.Key, cancellationToken);
                if (!attr.IsKey)
                {
                    other[entry.Key] = entry.Value;
                }
            }
        }

        return other;
    }

    /// <summary>
    /// 获取属性(按产品类型 + SKU范围 + 属性代码)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributeCode">属性代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>属性</returns>
    private async Task<AttributeEntity> GetAttributeAsync(string productType, string attributeCode, CancellationToken cancellationToken)
    {
        AttributeEntity? attribute = await _attributeRepository.GetByProductTypeAndScopeAndCodeAsync(productType, AttributeScopeEnum.Sku, attributeCode, cancellationToken);
        if (attribute == null)
        {
            throw new UserFriendlyException("属性" + attributeCode + ":不存在!");
        }

        return attribute;
    }

    /// <summary>
    /// 辅料: 生成一个无关键属性的SKU(如已存在则跳过)
    /// </summary>
    /// <param name="spu">产品</param>
    /// <param name="existing">已有SKU列表</param>
    /// <param name="toSave">待持久化列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task CreateForAccessoryAsync(Spu spu, List<Sku> existing, List<Sku> toSave, CancellationToken cancellationToken)
    {
        string signature = Sku.CalculateSignature(spu.Id, null);
        bool exists = existing.Any(x => signature == x.AttributesSignature);
        if (!exists)
        {
            Sku model = await BuildSkuAsync(spu.ProductType, spu.Id, null, null, null, null, null, null, null, null, cancellationToken);
            toSave.Add(model);
        }
    }

    /// <summary>
    /// 成品: 按SPH/CYL组合生成, 超出区间的旧SKU标记删除
    /// </summary>
    /// <param name="spu">产品</param>
    /// <param name="existing">已有SKU列表</param>
    /// <param name="toSave">待持久化列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task CreateForEndProductAsync(Spu spu, List<Sku> existing, List<Sku> toSave, CancellationToken cancellationToken)
    {
        int minSph = GetIntAttr(spu, "MinSPH");
        int maxSph = GetIntAttr(spu, "MaxSPH");
        int minCyl = GetIntAttr(spu, "MinCYL");
        int maxCyl = GetIntAttr(spu, "MaxCYL");

        for (int sph = minSph; sph <= maxSph; sph += 25)
        {
            for (int cyl = minCyl; cyl <= maxCyl; cyl += 25)
            {
                SortedDictionary<string, object?> dict = new SortedDictionary<string, object?>();
                dict["SPH"] = sph;
                dict["CYL"] = cyl;
                string signature = Sku.CalculateSignature(spu.Id, dict);
                bool exists = existing.Any(x => signature == x.AttributesSignature);
                if (!exists)
                {
                    Sku model = await BuildSkuAsync(spu.ProductType, spu.Id, null, null, null, null, null, null, dict, null, cancellationToken);
                    toSave.Add(model);
                }
            }
        }

        int skuMinSph = existing.Count == 0 ? 0 : MinKeyAttr(existing, "SPH");
        int skuMaxSph = existing.Count == 0 ? 0 : MaxKeyAttr(existing, "SPH");
        int skuMinCyl = existing.Count == 0 ? 0 : MinKeyAttr(existing, "CYL");
        int skuMaxCyl = existing.Count == 0 ? 0 : MaxKeyAttr(existing, "CYL");

        if (minSph > skuMinSph)
        {
            for (int sph = skuMinSph; sph < minSph; sph += 25)
            {
                for (int cyl = skuMinCyl; cyl <= skuMaxCyl; cyl += 25)
                {
                    RemoveByKey(existing, spu.Id, sph, cyl, toSave);
                }
            }
        }

        if (maxSph < skuMaxSph)
        {
            for (int sph = skuMaxSph; sph > maxSph; sph -= 25)
            {
                for (int cyl = skuMinCyl; cyl <= skuMaxCyl; cyl += 25)
                {
                    RemoveByKey(existing, spu.Id, sph, cyl, toSave);
                }
            }
        }

        if (minCyl > skuMinCyl)
        {
            for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25)
            {
                for (int cyl = skuMinCyl; cyl < minCyl; cyl += 25)
                {
                    RemoveByKey(existing, spu.Id, sph, cyl, toSave);
                }
            }
        }

        if (maxCyl < skuMaxCyl)
        {
            for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25)
            {
                for (int cyl = skuMaxCyl; cyl > maxCyl; cyl -= 25)
                {
                    RemoveByKey(existing, spu.Id, sph, cyl, toSave);
                }
            }
        }
    }

    /// <summary>
    /// 按SPH/CYL找到对应SKU, 标记删除并加入待持久化列表
    /// </summary>
    /// <param name="existing">已有SKU列表</param>
    /// <param name="spuId">SpuId</param>
    /// <param name="sph">球镜度</param>
    /// <param name="cyl">柱镜度</param>
    /// <param name="toSave">待持久化列表</param>
    private static void RemoveByKey(List<Sku> existing, long spuId, int sph, int cyl, List<Sku> toSave)
    {
        SortedDictionary<string, object?> dict = new SortedDictionary<string, object?>();
        dict["SPH"] = sph;
        dict["CYL"] = cyl;
        string signature = Sku.CalculateSignature(spuId, dict);
        Sku? model = existing.FirstOrDefault(x => signature == x.AttributesSignature);
        if (model != null)
        {
            model.Delete();
            toSave.Add(model);
        }
    }

    /// <summary>
    /// 读取Spu属性里的整数值, 缺失时抛异常
    /// </summary>
    /// <param name="spu">产品</param>
    /// <param name="key">属性键</param>
    /// <returns>属性整数值</returns>
    private static int GetIntAttr(Spu spu, string key)
    {
        object? value = spu.Attributes == null ? null : spu.Attributes.TryGetValue(key, out object? v) ? v : null;
        if (value == null)
        {
            throw new UserFriendlyException("产品属性[" + key + "]不存在!");
        }

        return int.Parse(value.ToString()!);
    }

    /// <summary>
    /// 读取Sku关键属性里的整数最小值
    /// </summary>
    /// <param name="skus">SKU列表</param>
    /// <param name="key">属性键</param>
    /// <returns>最小值, 列表为空时返回0</returns>
    private static int MinKeyAttr(List<Sku> skus, string key)
    {
        return skus.Select(s => GetKeyAttrInt(s, key)).DefaultIfEmpty(0).Min();
    }

    /// <summary>
    /// 读取Sku关键属性里的整数最大值
    /// </summary>
    /// <param name="skus">SKU列表</param>
    /// <param name="key">属性键</param>
    /// <returns>最大值, 列表为空时返回0</returns>
    private static int MaxKeyAttr(List<Sku> skus, string key)
    {
        return skus.Select(s => GetKeyAttrInt(s, key)).DefaultIfEmpty(0).Max();
    }

    /// <summary>
    /// 读取Sku关键属性里的整数值, 缺失返回0
    /// </summary>
    /// <param name="sku">SKU</param>
    /// <param name="key">属性键</param>
    /// <returns>属性整数值, 缺失或格式错误时返回0</returns>
    private static int GetKeyAttrInt(Sku sku, string key)
    {
        object? value = sku.KeyAttributes == null ? null : sku.KeyAttributes.TryGetValue(key, out object? v) ? v : null;
        return value == null ? 0 : int.Parse(value.ToString()!);
    }

    /// <summary>
    /// 属性拆分结果
    /// </summary>
    /// <param name="Key">关键属性</param>
    /// <param name="Other">非关键属性</param>
    private sealed record SplitAttributes(SortedDictionary<string, object?> Key, SortedDictionary<string, object?> Other);
}
