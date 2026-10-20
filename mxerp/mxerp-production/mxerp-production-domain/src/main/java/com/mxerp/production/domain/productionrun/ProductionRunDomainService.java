package com.mxerp.production.domain.productionrun;

import com.mxerp.production.domain.workorder.WorkOrder;
import com.mxerp.production.domain.workorder.WorkOrderOperation;
import com.mxerp.production.domain.workorder.WorkOrderOperationStatusEnum;
import com.mxerp.production.domain.workorder.WorkOrderRepository;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 炉号单领域服务，负责跨聚合的资格判定
 *
 * <p>聚合根管得了自己这一炉（行、口径、摊回），管不到别人：一道工单工序存不存在、属不属于这张工单、是不是
 * 本炉那道工序、做完了没有、有没有被别炉占着——这些要读工单聚合，所以收在这里。
 *
 * <p>这里不判「能不能合」：容量上限、相容规则属排产与工艺主数据，等那条线要做时再长，成本归集这一步只需要
 * 「这一炉里各单占多少」这个事实。
 */
@NullMarked
public class ProductionRunDomainService {

  /** 炉号单工厂 */
  private final ProductionRunFactory productionRunFactory;

  /** 炉号单仓储 */
  private final ProductionRunRepository productionRunRepository;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /**
   * 构造器注入
   *
   * @param productionRunFactory 炉号单工厂
   * @param productionRunRepository 炉号单仓储
   * @param workOrderRepository 工单仓储
   */
  public ProductionRunDomainService(
      ProductionRunFactory productionRunFactory,
      ProductionRunRepository productionRunRepository,
      WorkOrderRepository workOrderRepository) {
    this.productionRunFactory = productionRunFactory;
    this.productionRunRepository = productionRunRepository;
    this.workOrderRepository = workOrderRepository;
  }

  /**
   * 创建炉号单（草稿）
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @param runTime 炉次时间（可空）
   * @param remark 备注（可空）
   * @param number 炉号（传空串表示由流水号监听器分配）
   * @return 炉号单聚合
   * @throws DomainException 炉号重复、分配标准非法时抛出
   */
  public ProductionRun create(
      Long operationId,
      Long workCenterId,
      String allocationBasis,
      @Nullable Long occupyUnitId,
      @Nullable LocalDateTime runTime,
      @Nullable String remark,
      String number) {
    return productionRunFactory.create(
        operationId, workCenterId, allocationBasis, occupyUnitId, runTime, remark, number);
  }

  /**
   * 把一张工单的某道工序请进这一炉
   *
   * <p>快照取自工单：单号、SPU、SKU 都在工单上，行上那份是给成本还原与追溯用的冗余，不能让调用方随手填一份跟
   * 工单不一致的。工序属不属于这张单、做完了没有、有没有被别炉占着，都在这一步查——等到过账才发现冲突，人已经
   * 白配了半天炉。
   *
   * @param id 炉号单Id
   * @param workOrderId 工单Id
   * @param workOrderOperationId 工单工序Id
   * @param occupyQuantity 占用量（按表头占用单位计）
   * @param allocationWeight 分摊权重（可空，取占用量）
   * @param remark 备注（可空）
   * @return 炉号单聚合
   * @throws DomainException 单据不存在或不是草稿、工单或工序不存在、工序已完工、工序被别炉占用时抛出
   */
  public ProductionRun addLine(
      long id,
      Long workOrderId,
      Long workOrderOperationId,
      BigDecimal occupyQuantity,
      @Nullable BigDecimal allocationWeight,
      @Nullable String remark) {
    ProductionRun model = getExist(id);
    WorkOrder workOrder = requireWorkOrder(workOrderId);
    WorkOrderOperation operation = requireOperation(workOrder, workOrderOperationId);
    if (!Objects.equals(operation.getOperationId(), model.getOperationId())) {
      throw new DomainException(
          "工单" + workOrder.getNumber() + "的这道工序不是本炉的工序，不能合到一炉");
    }
    if (isOperationFinished(operation)) {
      throw new DomainException(
          "工单" + workOrder.getNumber() + "的工序" + operation.getOperationName() + "已完工，不用记账");
    }
    ProductionRunLine occupied =
        productionRunRepository.getLiveLineByWorkOrderOperationId(
            workOrderOperationId, model.getId());
    if (occupied != null) {
      throw new DomainException(
          "工单" + workOrder.getNumber() + "的这道工序已在炉号" + occupied.getRunId() + "中");
    }
    model.addLine(
        workOrderId,
        workOrderOperationId,
        workOrder.getNumber(),
        workOrder.getProductId(),
        workOrder.getVariantId(),
        occupyQuantity,
        allocationWeight,
        remark);
    return model;
  }

  /**
   * 查一张工单当前进有哪些炉
   *
   * <p>改工单数量或取消工单之前要先问这一句：单上的工序被哪几炉记着账，就得先把那一行提出来或改小，而不是
   * 直接改单。作废与红冲的炉号单里的行已软删，不占位。
   *
   * @param workOrderId 工单Id
   * @return 炉号单行列表
   */
  public List<ProductionRunLine> getOpenLines(Long workOrderId) {
    return productionRunRepository.getLiveLinesByWorkOrderId(workOrderId);
  }

  // ==================== 私有 ====================

  /**
   * 取炉号单，不存在则抛异常
   *
   * @param id 炉号单Id
   * @return 炉号单聚合
   * @throws DomainException 不存在时抛出
   */
  private ProductionRun getExist(long id) {
    ProductionRun model = productionRunRepository.get(id);
    if (model == null) {
      throw new DomainException("炉号单" + id + "不存在!");
    }
    return model;
  }

  /**
   * 取工单，不存在则抛异常
   *
   * @param workOrderId 工单Id
   * @return 工单聚合
   * @throws DomainException 不存在时抛出
   */
  private WorkOrder requireWorkOrder(Long workOrderId) {
    WorkOrder workOrder = workOrderRepository.get(workOrderId);
    if (workOrder == null) {
      throw new DomainException("工单" + workOrderId + "不存在!");
    }
    if (workOrder.isClosed() || workOrder.isCancelled()) {
      throw new DomainException("工单" + workOrder.getNumber() + "已结束，不用记账");
    }
    return workOrder;
  }

  /**
   * 在工单里找那道工序
   *
   * @param workOrder 工单聚合
   * @param workOrderOperationId 工单工序Id
   * @return 工单工序
   * @throws DomainException 工序不属于这张工单时抛出
   */
  private WorkOrderOperation requireOperation(WorkOrder workOrder, Long workOrderOperationId) {
    return workOrder.getWorkOrderOperations().stream()
        .filter(i -> Objects.equals(i.getId(), workOrderOperationId))
        .findFirst()
        .orElseThrow(
            () ->
                new DomainException(
                    "工单" + workOrder.getNumber() + "的工序" + workOrderOperationId + "不存在!"));
  }

  /**
   * 工序是否已经结束（完工或跳过）：做完了的工序没有可以再摊的
   *
   * @param operation 工单工序
   * @return true 表示已结束
   */
  private boolean isOperationFinished(WorkOrderOperation operation) {
    WorkOrderOperationStatusEnum status =
        EnumUtils.fromCode(WorkOrderOperationStatusEnum.class, operation.getStatus());
    return status == WorkOrderOperationStatusEnum.COMPLETED
        || status == WorkOrderOperationStatusEnum.SKIPPED;
  }
}
