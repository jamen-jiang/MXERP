using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MXERP.Domain.Brands;
using MXERP.Domain.Currencies;
using MXERP.Domain.Units;
using MXERP.Domain.TaxRates;
using MXERP.Domain.Roles;
using MXERP.Domain.Positions;
using MXERP.Domain.Diameters;
using MXERP.Domain.Jobs;
using MXERP.Domain.Warehouses;
using MXERP.Domain.Suppliers;
using MXERP.Domain.Departments;
using MXERP.Domain.WorkSteps;
using MXERP.Domain.Ranks;
using MXERP.Domain.AuditLogs;
using MXERP.Domain.CommandLogs;
using MXERP.Domain.OperationLogs;
using MXERP.Domain.SystemLogs;
using MXERP.Domain.SignInLogs;
using MXERP.Domain.Attachments;
using MXERP.Domain.Emails;
using MXERP.Domain.Dictionaries;
using MXERP.Domain.Employees;
using MXERP.Domain.Privileges;
using MXERP.Domain.Apps;
using MXERP.Domain.Companies;
using MXERP.Domain.Stores;
using MXERP.Domain.Posts;
using MXERP.Domain.Users;
using MXERP.Domain.Attributes;
using AttributeEntity = MXERP.Domain.Attributes.Attribute;
using MXERP.Domain.Spus;
using MXERP.Domain.Skus;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.SalesSkus;
using MXERP.Domain.SalesSpuOtherAssignItems;
using MXERP.Domain.SpuBrands;
using MXERP.Domain.SpuCompanies;
using MXERP.Domain.Routings;
using MXERP.Domain.SerialNumbers;
using MXERP.Domain.CustomerInvoiceTitleNumbers;
using MXERP.Domain.EndProductCodeConfigs;
using MXERP.Domain.Processes;
using MXERP.Domain.RetailOrders;
using MXERP.Domain.RetailOrderWorkSteps;
using MXERP.Domain.CustomersSpus;
using MXERP.Domain.TripleInvoices;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.ExpressOrders;
using MXERP.Domain.ExternalOrderLogs;
using MXERP.Domain.ExternalRetailOrderTrackings;
using MXERP.Domain.OrderEvents;
using MXERP.Domain.ExternalLeaveStockOrders;
using MXERP.Domain.PackingSlips;
using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.Stocks;
using MXERP.Domain.PurchasePlans;
using MXERP.Domain.BulkOrders;
using MXERP.Domain.BulkOrderItems;
using MXERP.Domain.PurchaseBatches;
using MXERP.Domain.PurchaseOrders;
using MXERP.Domain.InventoryOrders;
using MXERP.Domain.PurchaseRequisitionOrders;
using MXERP.Domain.Works;
using MXERP.Domain.LeaveWarehouseOrders;
using MXERP.Domain.EnterStockOrders;
using MXERP.Domain.LeaveStockOrders;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore;

/// <summary>
/// MXERP 业务库上下文: 每个聚合根一个 DbSet, 仓储随之自动可用。
/// <para>表名/列类型/索引等细项走 Fluent 配置(见 Configurations/), 不写数据注解;
/// 审计字段与软删除过滤由 SnowDbContext 统一处理, 这里不重复。</para>
/// </summary>
public class MXERPDbContext : SnowDbContext
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="options">EF Core 选项</param>
    public MXERPDbContext(DbContextOptions<MXERPDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 品牌
    /// </summary>
    public DbSet<Brand> Brands => Set<Brand>();

    /// <summary>
    /// 货币
    /// </summary>
    public DbSet<Currency> Currencies => Set<Currency>();

    /// <summary>
    /// 单位
    /// </summary>
    public DbSet<Unit> Units => Set<Unit>();

    /// <summary>
    /// 税率
    /// </summary>
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();

    /// <summary>
    /// 角色
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// 职位
    /// </summary>
    public DbSet<Position> Positions => Set<Position>();

    /// <summary>
    /// 直径
    /// </summary>
    public DbSet<Diameter> Diameters => Set<Diameter>();

    /// <summary>
    /// 任务
    /// </summary>
    public DbSet<Job> Jobs => Set<Job>();

    /// <summary>
    /// 仓库
    /// </summary>
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    /// <summary>
    /// 供应商
    /// </summary>
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    /// <summary>
    /// 部门
    /// </summary>
    public DbSet<Department> Departments => Set<Department>();

    /// <summary>
    /// 工序
    /// </summary>
    public DbSet<WorkStep> WorkSteps => Set<WorkStep>();

    /// <summary>
    /// 职级
    /// </summary>
    public DbSet<Rank> Ranks => Set<Rank>();

    /// <summary>
    /// 审计日志
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// 命令日志
    /// </summary>
    public DbSet<CommandLog> CommandLogs => Set<CommandLog>();

    /// <summary>
    /// 操作日志
    /// </summary>
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();

    /// <summary>
    /// 系统日志
    /// </summary>
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

    /// <summary>
    /// 登录日志
    /// </summary>
    public DbSet<SignInLog> SignInLogs => Set<SignInLog>();

    /// <summary>
    /// 附件
    /// </summary>
    public DbSet<Attachment> Attachments => Set<Attachment>();

    /// <summary>
    /// 邮件
    /// </summary>
    public DbSet<Email> Emails => Set<Email>();

    /// <summary>
    /// 邮件附件
    /// </summary>
    public DbSet<EmailAttachment> EmailAttachments => Set<EmailAttachment>();

    /// <summary>
    /// 字典
    /// </summary>
    public DbSet<Dictionary> Dictionaries => Set<Dictionary>();

    /// <summary>
    /// 字典项
    /// </summary>
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();

    /// <summary>
    /// 员工
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// 权限
    /// </summary>
    public DbSet<Privilege> Privileges => Set<Privilege>();

    /// <summary>
    /// 应用
    /// </summary>
    public DbSet<App> Apps => Set<App>();

    /// <summary>
    /// 公司
    /// </summary>
    public DbSet<Company> Companies => Set<Company>();

    /// <summary>
    /// 公司联系人
    /// </summary>
    public DbSet<CompanyContact> CompanyContacts => Set<CompanyContact>();

    /// <summary>
    /// 门店
    /// </summary>
    public DbSet<Store> Stores => Set<Store>();

    /// <summary>
    /// 门店联系人
    /// </summary>
    public DbSet<StoreContact> StoreContacts => Set<StoreContact>();

    /// <summary>
    /// 岗位
    /// </summary>
    public DbSet<Post> Posts => Set<Post>();

    /// <summary>
    /// 岗位角色
    /// </summary>
    public DbSet<PostRole> PostRoles => Set<PostRole>();

    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// 用户岗位
    /// </summary>
    public DbSet<UserPost> UserPosts => Set<UserPost>();

    /// <summary>
    /// 用户角色
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    /// <summary>
    /// 属性
    /// </summary>
    public DbSet<AttributeEntity> Attributes => Set<AttributeEntity>();

    /// <summary>
    /// 属性项
    /// </summary>
    public DbSet<AttributeItem> AttributeItems => Set<AttributeItem>();

    /// <summary>
    /// 属性校验规则
    /// </summary>
    public DbSet<AttributeValidateRule> AttributeValidateRules => Set<AttributeValidateRule>();

    /// <summary>
    /// 产品
    /// </summary>
    public DbSet<Spu> Spus => Set<Spu>();

    /// <summary>
    /// SKU
    /// </summary>
    public DbSet<Sku> Skus => Set<Sku>();

    /// <summary>
    /// 销售产品
    /// </summary>
    public DbSet<SalesSpu> SalesSpus => Set<SalesSpu>();

    /// <summary>
    /// 销售产品分配项
    /// </summary>
    public DbSet<SalesSpuAssignItem> SalesSpuAssignItems => Set<SalesSpuAssignItem>();

    /// <summary>
    /// 销售产品工序
    /// </summary>
    public DbSet<SalesSpuWorkStep> SalesSpuWorkSteps => Set<SalesSpuWorkStep>();

    /// <summary>
    /// 销售产品路线
    /// </summary>
    public DbSet<SalesSpuRouting> SalesSpuRoutings => Set<SalesSpuRouting>();

    /// <summary>
    /// 销售SKU
    /// </summary>
    public DbSet<SalesSku> SalesSkus => Set<SalesSku>();

    /// <summary>
    /// 销售产品其他分配项
    /// </summary>
    public DbSet<SalesSpuOtherAssignItem> SalesSpuOtherAssignItems => Set<SalesSpuOtherAssignItem>();

    /// <summary>
    /// 产品品牌
    /// </summary>
    public DbSet<SpuBrand> SpuBrands => Set<SpuBrand>();

    /// <summary>
    /// 产品公司关联
    /// </summary>
    public DbSet<SpuCompany> SpuCompanies => Set<SpuCompany>();

    /// <summary>
    /// 工艺路线
    /// </summary>
    public DbSet<Routing> Routings => Set<Routing>();

    /// <summary>
    /// 路线工序行
    /// </summary>
    public DbSet<RoutingStep> RoutingSteps => Set<RoutingStep>();

    /// <summary>
    /// 流水号
    /// </summary>
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();

    /// <summary>
    /// 客户票号
    /// </summary>
    public DbSet<CustomerInvoiceTitleNumber> CustomerInvoiceTitleNumbers => Set<CustomerInvoiceTitleNumber>();

    /// <summary>
    /// 成品代码配置
    /// </summary>
    public DbSet<EndProductCodeConfig> EndProductCodeConfigs => Set<EndProductCodeConfig>();

    /// <summary>
    /// 工序
    /// </summary>
    public DbSet<Process> Processes => Set<Process>();

    /// <summary>
    /// 散镜订单
    /// </summary>
    public DbSet<RetailOrder> RetailOrders => Set<RetailOrder>();

    /// <summary>
    /// 散镜订单项
    /// </summary>
    public DbSet<RetailOrderItem> RetailOrderItems => Set<RetailOrderItem>();

    /// <summary>
    /// 散镜订单跟踪
    /// </summary>
    public DbSet<RetailOrderTracking> RetailOrderTrackings => Set<RetailOrderTracking>();

    /// <summary>
    /// 散镜订单工作步骤
    /// </summary>
    public DbSet<RetailOrderWorkStep> RetailOrderWorkSteps => Set<RetailOrderWorkStep>();

    /// <summary>
    /// 客人镜种
    /// </summary>
    public DbSet<CustomerSpu> CustomerSpus => Set<CustomerSpu>();

    /// <summary>
    /// 三联发票
    /// </summary>
    public DbSet<TripleInvoice> TripleInvoices => Set<TripleInvoice>();

    /// <summary>
    /// 出库批次
    /// </summary>
    public DbSet<LeaveStockBatch> LeaveStockBatches => Set<LeaveStockBatch>();

    /// <summary>
    /// 出库批次项
    /// </summary>
    public DbSet<LeaveStockBatchItem> LeaveStockBatchItems => Set<LeaveStockBatchItem>();

    /// <summary>
    /// 出库批次工序
    /// </summary>
    public DbSet<LeaveStockBatchWorkStep> LeaveStockBatchWorkSteps => Set<LeaveStockBatchWorkStep>();

    /// <summary>
    /// 出库批次跟踪
    /// </summary>
    public DbSet<LeaveStockBatchTracking> LeaveStockBatchTrackings => Set<LeaveStockBatchTracking>();

    /// <summary>
    /// 出库计划
    /// </summary>
    public DbSet<LeaveStockPlan> LeaveStockPlans => Set<LeaveStockPlan>();

    /// <summary>
    /// 出库计划项
    /// </summary>
    public DbSet<LeaveStockPlanItem> LeaveStockPlanItems => Set<LeaveStockPlanItem>();

    /// <summary>
    /// 库存
    /// </summary>
    public DbSet<Stock> Stocks => Set<Stock>();

    /// <summary>
    /// 快递单
    /// </summary>
    public DbSet<ExpressOrder> ExpressOrders => Set<ExpressOrder>();

    /// <summary>
    /// 快递单项
    /// </summary>
    public DbSet<ExpressOrderItem> ExpressOrderItems => Set<ExpressOrderItem>();

    /// <summary>
    /// 外部订单日志
    /// </summary>
    public DbSet<ExternalOrderLog> ExternalOrderLogs => Set<ExternalOrderLog>();

    /// <summary>
    /// 外部散镜订单跟踪
    /// </summary>
    public DbSet<ExternalRetailOrderTracking> ExternalRetailOrderTrackings => Set<ExternalRetailOrderTracking>();

    /// <summary>
    /// 采购计划
    /// </summary>
    public DbSet<PurchasePlan> PurchasePlans => Set<PurchasePlan>();

    /// <summary>
    /// 采购计划项
    /// </summary>
    public DbSet<PurchasePlanItem> PurchasePlanItems => Set<PurchasePlanItem>();

    /// <summary>
    /// 订单事件
    /// </summary>
    public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();

    /// <summary>
    /// 订单事件绑定的组织机构
    /// </summary>
    public DbSet<OrderEventDepartment> OrderEventDepartments => Set<OrderEventDepartment>();

    /// <summary>
    /// 订单事件项
    /// </summary>
    public DbSet<OrderEventItem> OrderEventItems => Set<OrderEventItem>();

    /// <summary>
    /// 订单事件项公司
    /// </summary>
    public DbSet<OrderEventItemCompany> OrderEventItemCompanies => Set<OrderEventItemCompany>();

    /// <summary>
    /// 装箱单
    /// </summary>
    public DbSet<PackingSlip> PackingSlips => Set<PackingSlip>();

    /// <summary>
    /// 外部出库单
    /// </summary>
    public DbSet<ExternalLeaveStockOrder> ExternalLeaveStockOrders => Set<ExternalLeaveStockOrder>();

    /// <summary>
    /// 外部出库单跟踪
    /// </summary>
    public DbSet<ExternalLeaveStockOrderTracking> ExternalLeaveStockOrderTrackings => Set<ExternalLeaveStockOrderTracking>();

    /// <summary>
    /// 批量订单
    /// </summary>
    public DbSet<BulkOrder> BulkOrders => Set<BulkOrder>();

    /// <summary>
    /// 批量订单项
    /// </summary>
    public DbSet<BulkOrderItem> BulkOrderItems => Set<BulkOrderItem>();

    /// <summary>
    /// 批量订单项销售SKU行
    /// </summary>
    public DbSet<BulkOrderItemSalesSku> BulkOrderItemSalesSkus => Set<BulkOrderItemSalesSku>();

    /// <summary>
    /// 采购批次
    /// </summary>
    public DbSet<PurchaseBatch> PurchaseBatches => Set<PurchaseBatch>();

    /// <summary>
    /// 采购批次项
    /// </summary>
    public DbSet<PurchaseBatchItem> PurchaseBatchItems => Set<PurchaseBatchItem>();

    /// <summary>
    /// 采购单
    /// </summary>
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    /// <summary>
    /// 采购单项
    /// </summary>
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    /// <summary>
    /// 采购单关联表
    /// </summary>
    public DbSet<PurchaseOrderRelation> PurchaseOrderRelations => Set<PurchaseOrderRelation>();

    /// <summary>
    /// 盘点单
    /// </summary>
    public DbSet<InventoryOrder> InventoryOrders => Set<InventoryOrder>();

    /// <summary>
    /// 盘点单项
    /// </summary>
    public DbSet<InventoryOrderItem> InventoryOrderItems => Set<InventoryOrderItem>();

    /// <summary>
    /// 申购单
    /// </summary>
    public DbSet<PurchaseRequisitionOrder> PurchaseRequisitionOrders => Set<PurchaseRequisitionOrder>();

    /// <summary>
    /// 申购单项
    /// </summary>
    public DbSet<PurchaseRequisitionOrderItem> PurchaseRequisitionOrderItems => Set<PurchaseRequisitionOrderItem>();

    /// <summary>
    /// 工作
    /// </summary>
    public DbSet<Work> Works => Set<Work>();

    /// <summary>
    /// 工作日志
    /// </summary>
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();

    /// <summary>
    /// 出仓单
    /// </summary>
    public DbSet<LeaveWarehouseOrder> LeaveWarehouseOrders => Set<LeaveWarehouseOrder>();

    /// <summary>
    /// 出仓单项
    /// </summary>
    public DbSet<LeaveWarehouseOrderItem> LeaveWarehouseOrderItems => Set<LeaveWarehouseOrderItem>();

    /// <summary>
    /// 入库单
    /// </summary>
    public DbSet<EnterStockOrder> EnterStockOrders => Set<EnterStockOrder>();

    /// <summary>
    /// 入库单项
    /// </summary>
    public DbSet<EnterStockOrderItem> EnterStockOrderItems => Set<EnterStockOrderItem>();

    /// <summary>
    /// 入库单关联
    /// </summary>
    public DbSet<EnterStockOrderRelation> EnterStockOrderRelations => Set<EnterStockOrderRelation>();

    /// <summary>
    /// 出库单
    /// </summary>
    public DbSet<LeaveStockOrder> LeaveStockOrders => Set<LeaveStockOrder>();

    /// <summary>
    /// 出库单项
    /// </summary>
    public DbSet<LeaveStockOrderItem> LeaveStockOrderItems => Set<LeaveStockOrderItem>();

    /// <summary>
    /// 出库单关联
    /// </summary>
    public DbSet<LeaveStockOrderRelation> LeaveStockOrderRelations => Set<LeaveStockOrderRelation>();

    /// <inheritdoc />
    protected override void ConfigureByConvention(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MXERPDbContext).Assembly);

        // 枚举一律按字符串入库(与既有列类型保持一致), 无需逐个配置写 HasConversion。
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                Type clrType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
                if (clrType.IsEnum)
                {
                    Type converterType = typeof(EnumToStringConverter<>).MakeGenericType(clrType);
                    property.SetValueConverter((ValueConverter)Activator.CreateInstance(converterType)!);
                }
            }
        }
    }
}
