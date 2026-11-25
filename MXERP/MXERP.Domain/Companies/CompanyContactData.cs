namespace MXERP.Domain.Companies;

/// <summary>
/// 公司联系人数据
/// </summary>
/// <param name="Name">姓名</param>
/// <param name="Telephone">电话(可空)</param>
/// <param name="Area">地区(可空)</param>
/// <param name="Address">地址(可空)</param>
public sealed record CompanyContactData(string Name, string? Telephone, string? Area, string? Address);
