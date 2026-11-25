using Snow.Domain.Repositories;

namespace MXERP.Domain.AuditLogs;

/// <summary>
/// 审计日志仓储接口
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
}
