namespace MXERP.Domain.Stores;

/// <summary>
/// 门店工厂接口
/// </summary>
public interface IStoreFactory
{
    /// <summary>
    /// 创建门店
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
    Task<Store> CreateAsync(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default);
}
