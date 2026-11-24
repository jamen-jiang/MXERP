using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Brands;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore;

/// <summary>
/// MXERP 业务库上下文: 每个聚合根一个 DbSet, 仓储随之自动可用。
/// <para>表名/列类型/索引等细项走 Fluent 配置(见 Configurations/), 不写数据注解;
/// 审计字段与软删除过滤由 SnowDbContext 统一处理, 这里不重复。</para>
/// </summary>
public class MXERPDbContext : SnowDbContext
{
    /// <summary>构造器</summary>
    /// <param name="options">EF Core 选项</param>
    public MXERPDbContext(DbContextOptions<MXERPDbContext> options)
        : base(options)
    {
    }

    /// <summary>品牌</summary>
    public DbSet<Brand> Brands => Set<Brand>();

    /// <inheritdoc />
    protected override void ConfigureByConvention(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MXERPDbContext).Assembly);
    }
}
