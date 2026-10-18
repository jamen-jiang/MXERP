package com.mxerp.production.domain.productionreport;

import com.mxerp.production.domain.workorder.WorkOrder;
import com.mxerp.production.domain.workorder.WorkOrderOperation;
import com.mxerp.production.domain.workorder.WorkOrderRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 生产报工单领域服务
 *
 * <p>提交报工要同时动两个聚合：报工单自己置为已提交，工单工序累加良品/报废/工时。这是跨聚合的
 * 一致性动作，所以放在领域服务里编排，而不是让报工单去反向修改工单。
 *
 * <p>创建报工单只需要三个业务事实：哪道工序、出了多少良品、坏了多少。工单号、工序名称、工作中心
 * 这些快照由本服务从工单上抄下来，操作人沿用工序开工时记的那位，工时不填就按工序标准时间算。
 */
@NullMarked
public class ProductionReportDomainService {

  /** 工时保留位数，与时间列 decimal(18, 2) 对齐 */
  private static final int MINUTE_SCALE = 2;

  /** 报工单工厂 */
  private final ProductionReportFactory productionReportFactory;

  /** 报工单仓储 */
  private final ProductionReportRepository productionReportRepository;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /**
   * 构造器注入
   *
   * @param productionReportFactory 报工单工厂
   * @param productionReportRepository 报工单仓储
   * @param workOrderRepository 工单仓储
   */
  public ProductionReportDomainService(
      ProductionReportFactory productionReportFactory,
      ProductionReportRepository productionReportRepository,
      WorkOrderRepository workOrderRepository) {
    this.productionReportFactory = productionReportFactory;
    this.productionReportRepository = productionReportRepository;
    this.workOrderRepository = workOrderRepository;
  }

  /**
   * 创建报工单（草稿）
   *
   * @param workOrderId 工单Id
   * @param workOrderOperationId 工单工序Id
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 本次工时（分钟），传空则按工序标准时间算
   * @param remark 备注（可空）
   * @return 报工单聚合
   * @throws DomainException 工单或工序行不存在、按批取时却没有标准批量时抛出
   */
  public ProductionReport create(
      Long workOrderId,
      Long workOrderOperationId,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable BigDecimal workTime,
      @Nullable String remark) {
    WorkOrder workOrder = requireWorkOrder(workOrderId);
    WorkOrderOperation operation = requireOperation(workOrder, workOrderOperationId);
    BigDecimal minutes = resolveWorkTime(operation, goodQuantity, scrapQuantity, workTime);
    // 传空串：单号交给 OrderAddedEvent 的监听器分配
    return productionReportFactory.create(
        workOrder.getId(),
        workOrder.getNumber(),
        operation.getId(),
        operation.getOperationId(),
        operation.getOperationName(),
        operation.getWorkCenterId(),
        operation.getWorkCenterName(),
        operation.getOperatorId(),
        minutes,
        goodQuantity,
        scrapQuantity,
        remark,
        "");
  }

  /**
   * 修改报工单（仅草稿可改）
   *
   * <p>工序行可以换，但只在这张工单内换；要挪到别的工单就删掉重开一张。
   *
   * @param id 报工单Id
   * @param workOrderOperationId 工单工序Id
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 本次工时（分钟），传空则按工序标准时间算
   * @param remark 备注（可空）
   * @return 报工单聚合
   * @throws DomainException 报工单或工序行不存在、按批取时却没有标准批量时抛出
   */
  public ProductionReport modify(
      long id,
      Long workOrderOperationId,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable BigDecimal workTime,
      @Nullable String remark) {
    ProductionReport model = getExist(id);
    WorkOrder workOrder = requireWorkOrder(model.getWorkOrderId());
    WorkOrderOperation operation = requireOperation(workOrder, workOrderOperationId);
    BigDecimal minutes = resolveWorkTime(operation, goodQuantity, scrapQuantity, workTime);
    model.modify(
        workOrder.getId(),
        workOrder.getNumber(),
        operation.getId(),
        operation.getOperationId(),
        operation.getOperationName(),
        operation.getWorkCenterId(),
        operation.getWorkCenterName(),
        operation.getOperatorId(),
        minutes,
        goodQuantity,
        scrapQuantity,
        remark);
    return model;
  }

  /**
   * 提交报工单：置为已提交，并累加到工单工序
   *
   * <p>先让工单侧校验通过再改自己的状态——反过来的话，工单侧一抛异常就会留下一张「已提交但没汇总」
   * 的报工单。
   *
   * <p>标了「报工倒冲」的用料不在这里触发：它要生成一张领料单，那张单得由应用层保存，与库存动账同理。
   * 应用层在这一步之后调 {@code BackflushIssueDomainService.generate}，触发传报工倒冲、产出量传良品加报废即可；
   * 发料仓取用料行上快照的那个（来自 BOM 行的发料仓）。
   *
   * @param id 报工单Id
   * @return 报工单聚合
   * @throws DomainException 报工单或工单不存在、工序未开工、报工数超过投入数时抛出
   */
  public ProductionReport submit(long id) {
    ProductionReport model = getExist(id);
    if (model.isSubmitted()) {
      throw new DomainException("报工单" + model.getNumber() + "已提交，不能重复提交");
    }

    WorkOrder workOrder = workOrderRepository.get(model.getWorkOrderId());
    if (workOrder == null) {
      throw new DomainException("工单" + model.getWorkOrderId() + "不存在!");
    }
    workOrder.reportOperation(
        model.getWorkOrderOperationId(),
        model.getGoodQuantity(),
        model.getScrapQuantity(),
        model.getWorkTime());

    model.submit();
    return model;
  }

  /**
   * 红冲报工单：置为已红冲，并把报工数从工单工序里减回去
   *
   * <p>与提交同样的顺序：先让工单侧校验通过再改自己的状态——反过来的话，工单侧一抛异常就会留下一张
   * 「已红冲但没减数」的报工单。
   *
   * <p>报工单只有这一条撤销路：提交那一下就已经计入工单工序，没有「报了还没生效」的中间态，所以没有作废之说，
   * 也不像领料单、入库单那样另有一张红字单要开——数全在工单工序的汇总列上，减回去就完了。
   *
   * <p>红冲只回到工单工序的汇总数：这张报工单触发过的报工倒冲不会跟着退，要退料由应用层做红字领料。
   *
   * @param id 报工单Id
   * @return 报工单聚合
   * @throws DomainException 报工单或工单不存在、非已提交状态、后续工序已开工、或工单侧校验不过时抛出
   */
  public ProductionReport reverse(long id) {
    ProductionReport model = getExist(id);
    if (!model.isSubmitted()) {
      throw new DomainException("报工单" + model.getNumber() + "不是已提交状态，不能红冲");
    }

    WorkOrder workOrder = requireWorkOrder(model.getWorkOrderId());
    workOrder.unreportOperation(
        model.getWorkOrderOperationId(),
        model.getGoodQuantity(),
        model.getScrapQuantity(),
        model.getWorkTime());

    model.reverse();
    return model;
  }

  /**
   * 按工单查询已提交的报工单
   *
   * @param workOrderId 工单Id
   * @return 已提交的报工单列表
   */
  public List<ProductionReport> getSubmittedByWorkOrderId(Long workOrderId) {
    return productionReportRepository.getListByWorkOrderId(workOrderId).stream()
        .filter(ProductionReport::isSubmitted)
        .toList();
  }

  // ==================== 私有：取数与工时派生 ====================

  /**
   * 取工单，不存在则抛异常
   *
   * @param workOrderId 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  private WorkOrder requireWorkOrder(Long workOrderId) {
    WorkOrder workOrder = workOrderRepository.get(workOrderId);
    if (workOrder == null) {
      throw new DomainException("工单" + workOrderId + "不存在!");
    }
    return workOrder;
  }

  /**
   * 在这张工单内取工序行
   *
   * @param workOrder 工单聚合
   * @param workOrderOperationId 工单工序Id
   * @return 工单工序行
   * @throws DomainException 工序行不属于这张工单时抛出
   */
  private WorkOrderOperation requireOperation(WorkOrder workOrder, Long workOrderOperationId) {
    return workOrder.getWorkOrderOperations().stream()
        .filter(operation -> operation.getId().equals(workOrderOperationId))
        .findFirst()
        .orElseThrow(
            () ->
                new DomainException(
                    "工单" + workOrder.getNumber() + "下没有Id为" + workOrderOperationId + "的工序行"));
  }

  /**
   * 定本次工时：现场填了就用现场的，没填就按工序标准时间算
   *
   * <p>产出量取良品加报废——报废件同样吃掉了加工时间。准备工时只在该工序还没有任何已提交报工时补一次，
   * 否则一道分三次报完的工序会把装夹调机算三遍。估算口径复用
   * {@code WorkOrderOperation#estimatedRunMinutes} 与 {@code estimatedSetupMinutes}，按批取时的工序
   * 不足一批也按整批的标准工时计。
   *
   * <p>「第一次」是看工序上已累计的报工量判的，所以两张都还是草稿时会各计一次准备工时；要精确到
   * 「开工即计准备工时」得给工序行加标志位，等成本那条线做细时再加。
   *
   * @param operation 工单工序行
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 现场填报工时（可空）
   * @return 本次工时（分钟）
   * @throws DomainException 按批取时却没有标准批量时抛出
   */
  private BigDecimal resolveWorkTime(
      WorkOrderOperation operation,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable BigDecimal workTime) {
    if (workTime != null) {
      return workTime;
    }
    BigDecimal output = goodQuantity.add(scrapQuantity);
    BigDecimal minutes = operation.estimatedRunMinutes(output);
    BigDecimal reported = operation.getCompletedQuantity().add(operation.getScrapQuantity());
    if (reported.signum() == 0) {
      minutes = minutes.add(operation.estimatedSetupMinutes(output));
    }
    return minutes.setScale(MINUTE_SCALE, RoundingMode.HALF_UP);
  }

  /**
   * 取报工单，不存在则抛异常
   *
   * @param id 报工单Id
   * @return 报工单聚合
   * @throws DomainException 报工单不存在时抛出
   */
  private ProductionReport getExist(long id) {
    ProductionReport model = productionReportRepository.get(id);
    if (model == null) {
      throw new DomainException("报工单" + id + "不存在!");
    }
    return model;
  }
}
