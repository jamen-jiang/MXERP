namespace MXERP.Domain.Apps;

/// <summary>
/// 应用生成结果
/// </summary>
/// <param name="App">应用</param>
/// <param name="Apps">子应用列表</param>
public sealed record AppGenerateResult(App App, List<App> Apps);
