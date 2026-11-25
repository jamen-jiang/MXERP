namespace MXERP.Domain.Stores;

/// <summary>
/// 门店领域服务接口(负责跨聚合编排和联系人同步)
/// </summary>
public interface IStoreDomainService
{
    /// <summary>
    /// 修改门店(按Id查询后修改)
    /// </summary>
    /// <param name="id">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">门店代码</param>
    /// <param name="name">门店名称</param>
    /// <param name="shortName">门店简称(可空)</param>
    /// <param name="number">门店编号(可空)</param>
    /// <param name="invoiceTitle">发票抬头(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="cityCode">区号代码(可空)</param>
    /// <param name="cityName">区号名称(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="logisticsCode">物流代码(可空)</param>
    /// <param name="logisticsName">物流名称(可空)</param>
    /// <param name="signer">代表人(可空)</param>
    /// <param name="convertCode">门店转换代码(可空)</param>
    /// <param name="address">门店地址(可空)</param>
    /// <param name="billingAddress">门店账户地址(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回门店</returns>
    Task<Store> ModifyAsync(long id, long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改门店(使用已有聚合)
    /// </summary>
    /// <param name="store">门店聚合</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">门店代码</param>
    /// <param name="name">门店名称</param>
    /// <param name="shortName">门店简称(可空)</param>
    /// <param name="number">门店编号(可空)</param>
    /// <param name="invoiceTitle">发票抬头(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="cityCode">区号代码(可空)</param>
    /// <param name="cityName">区号名称(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="logisticsCode">物流代码(可空)</param>
    /// <param name="logisticsName">物流名称(可空)</param>
    /// <param name="signer">代表人(可空)</param>
    /// <param name="convertCode">门店转换代码(可空)</param>
    /// <param name="address">门店地址(可空)</param>
    /// <param name="billingAddress">门店账户地址(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Store store, long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成: 门店存在则修改, 不存在则创建
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">门店代码</param>
    /// <param name="name">门店名称</param>
    /// <param name="shortName">门店简称(可空)</param>
    /// <param name="number">门店编号(可空)</param>
    /// <param name="invoiceTitle">发票抬头(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="cityCode">区号代码(可空)</param>
    /// <param name="cityName">区号名称(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="logisticsCode">物流代码(可空)</param>
    /// <param name="logisticsName">物流名称(可空)</param>
    /// <param name="signer">代表人(可空)</param>
    /// <param name="convertCode">门店转换代码(可空)</param>
    /// <param name="address">门店地址(可空)</param>
    /// <param name="billingAddress">门店账户地址(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回门店</returns>
    Task<Store> ImportGenerateAsync(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步联系人列表(按姓名匹配)
    /// </summary>
    /// <param name="store">门店</param>
    /// <param name="contacts">目标联系人列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncStoreContactsAsync(Store store, List<StoreContactData>? contacts, CancellationToken cancellationToken = default);
}
