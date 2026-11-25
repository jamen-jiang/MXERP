namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品领域服务接口
/// </summary>
public interface ISalesSpuDomainService
{
    /// <summary>
    /// 创建销售产品
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品聚合</returns>
    Task<SalesSpu> CreateAsync(string productType, long companyId, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成(存在则修改, 不存在则新建)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="workStepIds">工序Id列表</param>
    /// <param name="salesSpuAssignItems">分配项列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品聚合</returns>
    Task<SalesSpu> ImportGenerateAsync(string productType, long companyId, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? workStepIds, IReadOnlyCollection<SalesSpuAssignItemData>? salesSpuAssignItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改(按Id查询后修改)
    /// </summary>
    /// <param name="id">销售产品Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品聚合</returns>
    Task<SalesSpu> ModifyAsync(long id, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改(使用已有聚合)
    /// </summary>
    /// <param name="model">销售产品聚合</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(SalesSpu model, string code, string name, string? description, string? bulkOrderPacketInfo, string? retailOrderPacketInfo, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步分配项列表(输入里有则更新/新增, 没有则标记删除)
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="salesSpuAssignItems">分配项列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncAssignItemsAsync(SalesSpu salesSpu, IReadOnlyCollection<SalesSpuAssignItemData>? salesSpuAssignItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步工序列表(输入里有则保留, 没有则标记删除)
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="workStepIds">工序Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncWorkStepsAsync(SalesSpu salesSpu, IReadOnlyCollection<long>? workStepIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步路线列表(按产品Id业务标识匹配)
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="salesSpuRoutings">路线列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncRoutingsAsync(SalesSpu salesSpu, IReadOnlyCollection<SalesSpuRoutingData>? salesSpuRoutings, CancellationToken cancellationToken = default);
}
