using Snow.Domain.Repositories;

namespace MXERP.Domain.CommandLogs;

/// <summary>
/// 命令日志仓储接口
/// </summary>
public interface ICommandLogRepository : IRepository<CommandLog>
{
}
