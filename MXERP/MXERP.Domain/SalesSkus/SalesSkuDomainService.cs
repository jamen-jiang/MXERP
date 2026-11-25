using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Attributes;
using AttributeEntity = MXERP.Domain.Attributes.Attribute;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SalesSpus;

namespace MXERP.Domain.SalesSkus;

/// <summary>
/// 销售SKU领域服务
/// </summary>
[ScopedService]
public class SalesSkuDomainService : ISalesSkuDomainService
{
    private readonly ISalesSkuRepository _salesSkuRepository;
    private readonly ISalesSpuRepository _salesSpuRepository;
    private readonly IAttributeRepository _attributeRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSkuRepository">销售SKU仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="attributeRepository">属性仓储</param>
    public SalesSkuDomainService(ISalesSkuRepository salesSkuRepository, ISalesSpuRepository salesSpuRepository, IAttributeRepository attributeRepository)
    {
        _salesSkuRepository = salesSkuRepository;
        _salesSpuRepository = salesSpuRepository;
        _attributeRepository = attributeRepository;
    }

    /// <inheritdoc />
    public async Task<SalesSku> CreateAsync(string productType, long salesSpuId, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        SplitAttributes split = await SplitAttributesAsync(productType, attributes, cancellationToken);
        return await BuildSalesSkuAsync(productType, salesSpuId, opc, split.Key, split.Other, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SalesSku> ImportGenerateAsync(string productType, long salesSpuId, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        SplitAttributes split = await SplitAttributesAsync(productType, attributes, cancellationToken);
        string signature = SalesSku.CalculateSignature(salesSpuId, split.Key);
        SalesSku? model = await _salesSkuRepository.GetByAttributesSignatureAsync(signature, cancellationToken);
        if (model == null)
        {
            model = await BuildSalesSkuAsync(productType, salesSpuId, opc, split.Key, split.Other, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, opc, attributes, cancellationToken);
        }

        return model;
    }

    /// <inheritdoc />
    public async Task<SalesSku> ModifyAsync(long id, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default)
    {
        SalesSku? model = await _salesSkuRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("产品Sku不存在!");
        }

        await ModifyAsync(model, opc, attributes, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task<List<SalesSku>> CreateFromSalesSpuAsync(long salesSpuId, CancellationToken cancellationToken = default)
    {
        SalesSpu? salesSpu = await _salesSpuRepository.FindAsync(salesSpuId, true, cancellationToken);
        if (salesSpu == null)
        {
            throw new UserFriendlyException("销售产品" + salesSpuId + "不存在!");
        }

        List<SalesSku> existing = await _salesSkuRepository.GetBySalesSpuIdAsync(salesSpu.Id, cancellationToken);
        List<SalesSku> toSave = new List<SalesSku>();

        if (ProductTypeEnum.Accessory == salesSpu.ProductType)
        {
            await CreateForAccessoryAsync(salesSpu, existing, toSave, cancellationToken);
        }
        else if (ProductTypeEnum.EndProduct == salesSpu.ProductType)
        {
            await CreateForEndProductAsync(salesSpu, existing, toSave, cancellationToken);
        }

        return toSave;
    }

    /// <summary>
    /// 构建销售SKU(含销售产品存在性、OPC唯一性、属性组合唯一性校验)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="keyAttributes">关键属性(已拆分)</param>
    /// <param name="otherAttributes">非关键属性(已拆分)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU</returns>
    private async Task<SalesSku> BuildSalesSkuAsync(string productType, long salesSpuId, string? opc, SortedDictionary<string, object?>? keyAttributes, SortedDictionary<string, object?>? otherAttributes, CancellationToken cancellationToken)
    {
        SalesSpu? salesSpu = await _salesSpuRepository.FindAsync(salesSpuId, true, cancellationToken);
        if (salesSpu == null)
        {
            throw new UserFriendlyException("销售产品" + salesSpuId + "不存在!");
        }

        if (!string.IsNullOrWhiteSpace(opc) && await _salesSkuRepository.IsExistByCompanyIdAndOpcExcludeIdAsync(salesSpu.CompanyId, opc, null, cancellationToken))
        {
            throw new UserFriendlyException("OPC:" + opc + "已存在!");
        }

        SalesSku model = new SalesSku(productType, salesSpu.CompanyId, salesSpuId, opc, keyAttributes, otherAttributes);

        if (await _salesSkuRepository.IsExistByAttributesSignatureAsync(model.AttributesSignature, cancellationToken))
        {
            throw new UserFriendlyException("相同属性组合的SalesSku已存在!");
        }

        return model;
    }

    /// <summary>
    /// 修改(使用已有聚合)
    /// </summary>
    /// <param name="model">销售SKU聚合</param>
    /// <param name="opc">Opc</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task ModifyAsync(SalesSku model, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(opc) && await _salesSkuRepository.IsExistByCompanyIdAndOpcExcludeIdAsync(model.CompanyId, opc, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("OPC:" + opc + "已存在!");
        }

        SortedDictionary<string, object?> otherAttributes = await FilterOtherAttributesAsync(model.ProductType, attributes, cancellationToken);
        model.Modify(opc, otherAttributes);
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
    /// 获取属性(按产品类型 + 销售SKU范围 + 属性代码)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributeCode">属性代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>属性</returns>
    private async Task<AttributeEntity> GetAttributeAsync(string productType, string attributeCode, CancellationToken cancellationToken)
    {
        AttributeEntity? attribute = await _attributeRepository.GetByProductTypeAndScopeAndCodeAsync(productType, AttributeScopeEnum.SalesSku, attributeCode, cancellationToken);
        if (attribute == null)
        {
            throw new UserFriendlyException("属性" + attributeCode + ":不存在!");
        }

        return attribute;
    }

    /// <summary>
    /// 辅料: 生成一个无关键属性的销售SKU(如已存在则跳过)
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="existing">已有销售SKU列表</param>
    /// <param name="toSave">待持久化列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task CreateForAccessoryAsync(SalesSpu salesSpu, List<SalesSku> existing, List<SalesSku> toSave, CancellationToken cancellationToken)
    {
        string signature = SalesSku.CalculateSignature(salesSpu.Id, null);
        bool exists = existing.Any(x => signature == x.AttributesSignature);
        if (!exists)
        {
            SalesSku model = await BuildSalesSkuAsync(salesSpu.ProductType, salesSpu.Id, null, null, null, cancellationToken);
            toSave.Add(model);
        }
    }

    /// <summary>
    /// 成品: 按SPH/CYL组合生成, 超出区间的旧销售SKU标记删除
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="existing">已有销售SKU列表</param>
    /// <param name="toSave">待持久化列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task CreateForEndProductAsync(SalesSpu salesSpu, List<SalesSku> existing, List<SalesSku> toSave, CancellationToken cancellationToken)
    {
        int minSph = GetIntAttr(salesSpu, "MinSPH");
        int maxSph = GetIntAttr(salesSpu, "MaxSPH");
        int minCyl = GetIntAttr(salesSpu, "MinCYL");
        int maxCyl = GetIntAttr(salesSpu, "MaxCYL");

        for (int sph = minSph; sph <= maxSph; sph += 25)
        {
            for (int cyl = minCyl; cyl <= maxCyl; cyl += 25)
            {
                SortedDictionary<string, object?> dict = new SortedDictionary<string, object?>();
                dict["SPH"] = sph;
                dict["CYL"] = cyl;
                string signature = SalesSku.CalculateSignature(salesSpu.Id, dict);
                bool exists = existing.Any(x => signature == x.AttributesSignature);
                if (!exists)
                {
                    SalesSku model = await BuildSalesSkuAsync(salesSpu.ProductType, salesSpu.Id, null, dict, null, cancellationToken);
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
                    RemoveByKey(existing, salesSpu.Id, sph, cyl, toSave);
                }
            }
        }

        if (maxSph < skuMaxSph)
        {
            for (int sph = skuMaxSph; sph > maxSph; sph -= 25)
            {
                for (int cyl = skuMinCyl; cyl <= skuMaxCyl; cyl += 25)
                {
                    RemoveByKey(existing, salesSpu.Id, sph, cyl, toSave);
                }
            }
        }

        if (minCyl > skuMinCyl)
        {
            for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25)
            {
                for (int cyl = skuMinCyl; cyl < minCyl; cyl += 25)
                {
                    RemoveByKey(existing, salesSpu.Id, sph, cyl, toSave);
                }
            }
        }

        if (maxCyl < skuMaxCyl)
        {
            for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25)
            {
                for (int cyl = skuMaxCyl; cyl > maxCyl; cyl -= 25)
                {
                    RemoveByKey(existing, salesSpu.Id, sph, cyl, toSave);
                }
            }
        }
    }

    /// <summary>
    /// 按SPH/CYL找到对应销售SKU, 标记删除并加入待持久化列表
    /// </summary>
    /// <param name="existing">已有销售SKU列表</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="sph">球镜度</param>
    /// <param name="cyl">柱镜度</param>
    /// <param name="toSave">待持久化列表</param>
    private static void RemoveByKey(List<SalesSku> existing, long salesSpuId, int sph, int cyl, List<SalesSku> toSave)
    {
        SortedDictionary<string, object?> dict = new SortedDictionary<string, object?>();
        dict["SPH"] = sph;
        dict["CYL"] = cyl;
        string signature = SalesSku.CalculateSignature(salesSpuId, dict);
        SalesSku? model = existing.FirstOrDefault(x => signature == x.AttributesSignature);
        if (model != null)
        {
            model.Delete();
            toSave.Add(model);
        }
    }

    /// <summary>
    /// 读取销售产品属性里的整数值, 缺失时抛异常
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="key">属性键</param>
    /// <returns>属性整数值</returns>
    private static int GetIntAttr(SalesSpu salesSpu, string key)
    {
        object? value = salesSpu.Attributes == null ? null : salesSpu.Attributes.TryGetValue(key, out object? v) ? v : null;
        if (value == null)
        {
            throw new UserFriendlyException("销售产品属性[" + key + "]不存在!");
        }

        return int.Parse(value.ToString()!);
    }

    /// <summary>
    /// 读取销售SKU关键属性里的整数最小值
    /// </summary>
    /// <param name="salesSkus">销售SKU列表</param>
    /// <param name="key">属性键</param>
    /// <returns>最小值, 列表为空时返回0</returns>
    private static int MinKeyAttr(List<SalesSku> salesSkus, string key)
    {
        return salesSkus.Select(s => GetKeyAttrInt(s, key)).DefaultIfEmpty(0).Min();
    }

    /// <summary>
    /// 读取销售SKU关键属性里的整数最大值
    /// </summary>
    /// <param name="salesSkus">销售SKU列表</param>
    /// <param name="key">属性键</param>
    /// <returns>最大值, 列表为空时返回0</returns>
    private static int MaxKeyAttr(List<SalesSku> salesSkus, string key)
    {
        return salesSkus.Select(s => GetKeyAttrInt(s, key)).DefaultIfEmpty(0).Max();
    }

    /// <summary>
    /// 读取销售SKU关键属性里的整数值, 缺失返回0
    /// </summary>
    /// <param name="salesSku">销售SKU</param>
    /// <param name="key">属性键</param>
    /// <returns>属性整数值, 缺失或格式错误时返回0</returns>
    private static int GetKeyAttrInt(SalesSku salesSku, string key)
    {
        object? value = salesSku.KeyAttributes == null ? null : salesSku.KeyAttributes.TryGetValue(key, out object? v) ? v : null;
        return value == null ? 0 : int.Parse(value.ToString()!);
    }

    /// <summary>
    /// 属性拆分结果
    /// </summary>
    /// <param name="Key">关键属性</param>
    /// <param name="Other">非关键属性</param>
    private sealed record SplitAttributes(SortedDictionary<string, object?> Key, SortedDictionary<string, object?> Other);
}
