using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Attachments;
using MXERP.Domain.Attributes;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品领域服务
/// </summary>
[ScopedService]
public class SalesSpuDomainService : ISalesSpuDomainService
{
    private readonly ISalesSpuRepository _salesSpuRepository;
    private readonly IAttributeRepository _attributeRepository;
    private readonly IAttachmentRepository _attachmentRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="attributeRepository">属性仓储</param>
    /// <param name="attachmentRepository">附件仓储</param>
    public SalesSpuDomainService(ISalesSpuRepository salesSpuRepository, IAttributeRepository attributeRepository, IAttachmentRepository attachmentRepository)
    {
        _salesSpuRepository = salesSpuRepository;
        _attributeRepository = attributeRepository;
        _attachmentRepository = attachmentRepository;
    }

    /// <inheritdoc />
    public async Task<SalesSpu> CreateAsync(string productType, long companyId, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default)
    {
        if (await _salesSpuRepository.IsExistByProductTypeAndCompanyIdAndCodeExcludeIdAsync(productType, companyId, code, null, cancellationToken))
        {
            throw new UserFriendlyException("销售产品代码" + code + "已存在!");
        }

        await ValidateAttributesAsync(productType, attributes, cancellationToken);
        return new SalesSpu(productType, companyId, code, name, description, bulkOrderPacketInfo, retailOrderPacketInfo, attributes, attachmentIds, markingAttachmentIds);
    }

    /// <inheritdoc />
    public async Task<SalesSpu> ImportGenerateAsync(string productType, long companyId, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? workStepIds, IReadOnlyCollection<SalesSpuAssignItemData>? salesSpuAssignItems, CancellationToken cancellationToken = default)
    {
        List<long> finalWorkStepIds = workStepIds == null ? new List<long>() : workStepIds.ToList();
        List<SalesSpuAssignItemData> finalAssignItems = salesSpuAssignItems == null ? new List<SalesSpuAssignItemData>() : salesSpuAssignItems.ToList();

        SalesSpu? model = await _salesSpuRepository.GetByProductTypeAndCompanyIdAndCodeAsync(productType, companyId, code, cancellationToken);
        if (model == null)
        {
            model = await CreateAsync(productType, companyId, code, name, description, bulkOrderPacketInfo, retailOrderPacketInfo, attributes, null, null, cancellationToken);
        }
        else
        {
            List<Attachment> attachments = await _attachmentRepository.GetByOwnerTypeAndOwnerIdAsync(AttachmentOwnerTypeEnum.SalesLens, model.Id, cancellationToken);
            List<Attachment> markingAttachments = await _attachmentRepository.GetByOwnerTypeAndOwnerIdAsync(AttachmentOwnerTypeEnum.SalesLensMarking, model.Id, cancellationToken);
            await ModifyAsync(model, code, name, description, bulkOrderPacketInfo, retailOrderPacketInfo, attributes, attachments.Select(x => x.Id).ToList(), markingAttachments.Select(x => x.Id).ToList(), cancellationToken);
        }

        await SyncAssignItemsAsync(model, finalAssignItems, cancellationToken);
        await SyncWorkStepsAsync(model, finalWorkStepIds, cancellationToken);

        return model;
    }

    /// <inheritdoc />
    public async Task<SalesSpu> ModifyAsync(long id, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default)
    {
        SalesSpu? model = await _salesSpuRepository.FindAsync(id, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("销售产品不存在!");
        }

        await ModifyAsync(model, code, name, description, bulkOrderPacketInfo, retailOrderPacketInfo, attributes, attachmentIds, markingAttachmentIds, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(SalesSpu model, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default)
    {
        if (await _salesSpuRepository.IsExistByProductTypeAndCompanyIdAndCodeExcludeIdAsync(model.ProductType, model.CompanyId, code, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("销售产品代码" + code + "已存在!");
        }

        await ValidateAttributesAsync(model.ProductType, attributes, cancellationToken);
        model.Modify(code, name, description, bulkOrderPacketInfo, retailOrderPacketInfo, attributes, attachmentIds, markingAttachmentIds);
    }

    /// <inheritdoc />
    public async Task SyncAssignItemsAsync(SalesSpu salesSpu, IReadOnlyCollection<SalesSpuAssignItemData>? salesSpuAssignItems, CancellationToken cancellationToken = default)
    {
        List<SalesSpuAssignItemData> inputs = salesSpuAssignItems == null ? new List<SalesSpuAssignItemData>() : salesSpuAssignItems.ToList();

        List<SalesSpuAssignItem> toRemoves = salesSpu.GetSalesSpuAssignItems()
            .Where(a => !inputs.Any(i => i.SpuId == a.SpuId && i.WarehouseId == a.WarehouseId))
            .ToList();
        foreach (SalesSpuAssignItem item in toRemoves)
        {
            salesSpu.DeleteAssignItem(item.Id);
        }

        Dictionary<(long?, long?), long> existingDict = new Dictionary<(long?, long?), long>();
        foreach (SalesSpuAssignItem item in salesSpu.GetSalesSpuAssignItems())
        {
            existingDict.TryAdd((item.SpuId, item.WarehouseId), item.Id);
        }

        foreach (SalesSpuAssignItemData item in inputs)
        {
            if (existingDict.TryGetValue((item.SpuId, item.WarehouseId), out long id))
            {
                salesSpu.ModifyAssignItem(id, item.SpuId ?? 0, item.WarehouseId ?? 0, item.Sequence ?? 0, item.IsPriorityCustomized);
            }
            else
            {
                salesSpu.AddAssignItem(item.SpuId ?? 0, item.WarehouseId ?? 0, item.Sequence ?? 0, item.IsPriorityCustomized);
            }
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SyncWorkStepsAsync(SalesSpu salesSpu, IReadOnlyCollection<long>? workStepIds, CancellationToken cancellationToken = default)
    {
        List<long> inputs = workStepIds == null ? new List<long>() : workStepIds.ToList();

        List<SalesSpuWorkStep> toRemoves = salesSpu.GetSalesSpuWorkSteps()
            .Where(a => !inputs.Contains(a.WorkStepId))
            .ToList();
        foreach (SalesSpuWorkStep item in toRemoves)
        {
            salesSpu.DeleteWorkStep(item.Id);
        }

        HashSet<long> existingWorkStepIds = salesSpu.GetSalesSpuWorkSteps().Select(x => x.WorkStepId).ToHashSet();
        foreach (long workStepId in inputs)
        {
            if (!existingWorkStepIds.Contains(workStepId))
            {
                salesSpu.AddWorkStep(workStepId);
            }
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SyncRoutingsAsync(SalesSpu salesSpu, IReadOnlyCollection<SalesSpuRoutingData>? salesSpuRoutings, CancellationToken cancellationToken = default)
    {
        List<SalesSpuRoutingData> inputs = salesSpuRoutings == null ? new List<SalesSpuRoutingData>() : salesSpuRoutings.ToList();

        List<SalesSpuRouting> toRemoves = salesSpu.GetSalesSpuRoutings()
            .Where(a => !inputs.Any(i => i.SpuId == a.SpuId))
            .ToList();
        foreach (SalesSpuRouting item in toRemoves)
        {
            salesSpu.DeleteRouting(item.Id);
        }

        Dictionary<long, long> existingDict = new Dictionary<long, long>();
        foreach (SalesSpuRouting item in salesSpu.GetSalesSpuRoutings())
        {
            if (!item.IsDeleted)
            {
                existingDict.TryAdd(item.SpuId, item.Id);
            }
        }

        foreach (SalesSpuRoutingData item in inputs)
        {
            if (item.SpuId == null)
            {
                continue;
            }

            if (existingDict.TryGetValue(item.SpuId.Value, out long id))
            {
                salesSpu.ModifyRouting(id, item.RoutingId ?? 0);
            }
            else
            {
                salesSpu.AddRouting(item.SpuId.Value, item.RoutingId ?? 0);
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 校验属性存在性
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task ValidateAttributesAsync(string productType, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken)
    {
        if (attributes == null)
        {
            return;
        }

        foreach (string attributeCode in attributes.Keys)
        {
            if (!await _attributeRepository.IsExistByProductTypeAndScopeAndCodeExcludeIdAsync(productType, AttributeScopeEnum.SalesSpu, attributeCode, null, cancellationToken))
            {
                throw new UserFriendlyException("属性" + attributeCode + ":不存在!");
            }
        }
    }
}
