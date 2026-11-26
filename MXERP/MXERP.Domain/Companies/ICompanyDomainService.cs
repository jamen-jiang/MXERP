namespace MXERP.Domain.Companies;

/// <summary>
/// 公司领域服务接口(负责跨聚合编排和联系人同步)
/// </summary>
public interface ICompanyDomainService
{
    /// <summary>
    /// 修改公司(按Id查询后修改)
    /// </summary>
    /// <param name="id">公司Id</param>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回公司</returns>
    Task<Company> ModifyAsync(long id, string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改公司(使用已有聚合)
    /// </summary>
    /// <param name="company">公司聚合</param>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Company company, string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成: 公司存在则修改, 不存在则创建
    /// </summary>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回公司</returns>
    Task<Company> ImportGenerateAsync(string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步联系人列表(按姓名匹配)
    /// </summary>
    /// <param name="company">公司</param>
    /// <param name="contacts">目标联系人列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncCompanyContactsAsync(Company company, List<CompanyContactData>? contacts, CancellationToken cancellationToken = default);
}
