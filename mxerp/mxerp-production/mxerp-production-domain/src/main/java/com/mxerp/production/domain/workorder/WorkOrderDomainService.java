package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.bom.Bom;
import com.mxerp.production.domain.bom.BomItem;
import com.mxerp.production.domain.bom.BomRepository;
import com.mxerp.production.domain.bom.BomStatusEnum;
import com.mxerp.production.domain.operation.Operation;
import com.mxerp.production.domain.operation.OperationRepository;
import com.mxerp.production.domain.routing.Routing;
import com.mxerp.production.domain.routing.RoutingOperation;
import com.mxerp.production.domain.routing.RoutingRepository;
import com.mxerp.production.domain.routing.RoutingStatusEnum;
import com.mxerp.production.domain.workcenter.WorkCenter;
import com.mxerp.production.domain.workcenter.WorkCenterRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.util.Comparator;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单领域服务
 *
 * <p>负责跨聚合编排：下达工单时把 BOM 展开成用料、把工艺路线快照成工序。这两件事都需要读别的聚合
 * （{@code Bom} / {@code Routing} / {@code Operation} / {@code WorkCenter}），所以放在领域服务而不是
 * {@code WorkOrder} 自己身上——聚合根只能管自己的不变量。
 */
@NullMarked
public class WorkOrderDomainService {

  /** 工单工厂 */
  private final WorkOrderFactory workOrderFactory;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /** BOM 仓储 */
  private final BomRepository bomRepository;

  /** 工艺路线仓储 */
  private final RoutingRepository routingRepository;

  /** 工序仓储 */
  private final OperationRepository operationRepository;

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /**
   * 构造器注入
   *
   * @param workOrderFactory 工单工厂
   * @param workOrderRepository 工单仓储
   * @param bomRepository BOM 仓储
   * @param routingRepository 工艺路线仓储
   * @param operationRepository 工序仓储
   * @param workCenterRepository 工作中心仓储
   */
  public WorkOrderDomainService(
      WorkOrderFactory workOrderFactory,
      WorkOrderRepository workOrderRepository,
      BomRepository bomRepository,
      RoutingRepository routingRepository,
      OperationRepository operationRepository,
      WorkCenterRepository workCenterRepository) {
    this.workOrderFactory = workOrderFactory;
    this.workOrderRepository = workOrderRepository;
    this.bomRepository = bomRepository;
    this.routingRepository = routingRepository;
    this.operationRepository = operationRepository;
    this.workCenterRepository = workCenterRepository;
  }

  /**
   * 创建工单（草稿）
   *
   * @param data 工单数据
   * @return 工单聚合
   */
  public WorkOrder create(WorkOrderData data) {
    return workOrderFactory.create(data);
  }

  /**
   * 修改工单（仅草稿可改）
   *
   * @param id 工单Id
   * @param data 工单数据
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  public WorkOrder modify(long id, WorkOrderData data) {
    WorkOrder model = getExist(id);
    model.modify(
        data.skuId(),
        data.quantity(),
        data.unitId(),
        data.sourceType(),
        data.sourceId(),
        data.sourceNumber(),
        data.bomId(),
        data.routingId(),
        data.plannedStartTime(),
        data.plannedEndTime(),
        data.wipWarehouseId(),
        data.fgWarehouseId(),
        data.scrapWarehouseId(),
        data.remark());
    return model;
  }

  /**
   * 下达工单：按 BOM 展开用料、按工艺路线快照工序
   *
   * <p>BOM 与路线都遵循「工单上指定优先，没指定才取生效版本」。路线不绑产品，所以工单没指定路线时
   * 必须由调用方在本次下达里传入，否则无法确定用哪条。
   *
   * @param id 工单Id
   * @param routingId 本次下达使用的工艺路线（可空，为空时取工单上已指定的路线）
   * @return 工单聚合
   * @throws DomainException 工单不存在、未指定路线、路线未发布时抛出
   */
  public WorkOrder release(long id, @Nullable Long routingId) {
    WorkOrder model = getExist(id);

    // 1. 定路线：本次传入优先，其次用工单上指定的
    Long resolvedRoutingId = routingId == null ? model.getRoutingId() : routingId;
    if (resolvedRoutingId == null) {
      throw new DomainException("工单未指定工艺路线，不能下达");
    }
    Routing routing = routingRepository.get(resolvedRoutingId);
    if (routing == null) {
      throw new DomainException("工艺路线" + resolvedRoutingId + "不存在!");
    }
    if (!RoutingStatusEnum.RELEASED.getCode().equals(routing.getStatus())) {
      throw new DomainException("工艺路线" + routing.getCode() + "未发布，不能下达工单");
    }

    // 2. 定 BOM：工单指定优先，其次取当前生效的已发布版本
    Long resolvedBomId = model.getBomId();
    Bom bom =
        resolvedBomId == null
            ? bomRepository.getReleasedBySkuId(model.getSkuId())
            : bomRepository.get(resolvedBomId);
    if (bom != null && !BomStatusEnum.RELEASED.getCode().equals(bom.getStatus())) {
      throw new DomainException("BOM 版本" + bom.getVersion() + "未发布，不能用于下达工单");
    }

    // 3. 快照工序、展开用料，逐条装进聚合
    snapshotOperations(model, routing);
    expandMaterials(model, bom);

    // 4. 固化
    model.release(bom == null ? null : bom.getId(), routing.getId(), routing.getVersion());
    return model;
  }

  /**
   * 取消工单
   *
   * @param id 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在、或当前状态不允许取消时抛出
   */
  public WorkOrder cancel(long id) {
    WorkOrder model = getExist(id);
    model.cancel();
    return model;
  }

  // ==================== 私有：展开与快照 ====================

  /**
   * 取工单，不存在则抛异常
   *
   * @param id 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  private WorkOrder getExist(long id) {
    WorkOrder model = workOrderRepository.get(id);
    if (model == null) {
      throw new DomainException("工单" + id + "不存在!");
    }
    return model;
  }

  /**
   * 把工艺路线快照成工单工序，逐条加进工单
   *
   * <p>工序名、工作中心名、可选取外协标识全部冗余存一份：路线或工序主数据后续改名改配置，在制工单不受影响。
   *
   * @param model 工单聚合
   * @param routing 工艺路线
   * @throws DomainException 引用的工序或工作中心已不存在时抛出
   */
  private void snapshotOperations(WorkOrder model, Routing routing) {
    List<RoutingOperation> items =
        routing.getRoutingOperations().stream()
            .filter(o -> !o.getIsDeleted())
            .sorted(Comparator.comparing(RoutingOperation::getSequence))
            .toList();

    for (RoutingOperation item : items) {
      Operation operation = operationRepository.get(item.getOperationId());
      if (operation == null) {
        throw new DomainException("工序" + item.getOperationId() + "不存在!");
      }
      WorkCenter workCenter = workCenterRepository.get(item.getWorkCenterId());
      if (workCenter == null) {
        throw new DomainException("工作中心" + item.getWorkCenterId() + "不存在!");
      }
      model.addOperation(
          item.getSequence(),
          operation.getId(),
          operation.getName(),
          Boolean.TRUE.equals(item.getIsOptional()),
          Boolean.TRUE.equals(item.getIsOutsourced()),
          workCenter.getId(),
          workCenter.getName(),
          item.getSetupTime(),
          item.getRunTime(),
          item.getQueueTime(),
          item.getStandardBatchSize(),
          item.getReworkOperationId(),
          item.getRemark());
    }
  }

  /**
   * 把 BOM 展开成工单用料，逐条加进工单
   *
   * <p>需求数量 = 单位用量 × 工单计划数量 × (1 + 损耗率)。产品没有 BOM 时什么都不加——纯加工类工单
   * 不消耗物料，允许下达。
   *
   * @param model 工单聚合
   * @param bom 展开所用的 BOM（可空，为空表示产品没有 BOM）
   */
  private void expandMaterials(WorkOrder model, @Nullable Bom bom) {
    if (bom == null) {
      return;
    }
    for (BomItem item : bom.getBomItems()) {
      if (item.getIsDeleted()) {
        continue;
      }
      model.addMaterial(
          item.getId(),
          item.getComponentSkuId(),
          item.requiredQuantity(model.getQuantity()),
          item.getUnitId(),
          null,
          item.getRemark());
    }
  }
}
