package com.mxerp.production.domain.workorder;

import com.mxerp.shared.domain.aggregate.OrderAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单（生产订单）
 *
 * <p>工单是生产的执行单元：一张工单做且只做一个 SKU 的一个数量。销售订单行可以拆多张工单（分批投产），
 * 库存补货与返工也可以直接开单，所以来源用 {@code sourceType + sourceId} 表达而不是绑死销售订单。
 *
 * <p>生命周期：{@code DRAFT → RELEASED → IN_PROGRESS → COMPLETED → CLOSED}，任意阶段可 {@code CANCELLED}。
 * 下达（{@link #release}）是唯一一次「定型」动作：按 BOM 展开用料、按工艺路线快照工序，之后路线改版不影响本单。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkOrder",
    indexes = {
      @Index(name = "IX_WorkOrder_Number", columnList = "Number"),
      @Index(name = "IX_WorkOrder_SkuId", columnList = "SkuId"),
      @Index(name = "IX_WorkOrder_Status", columnList = "Status"),
      @Index(name = "IX_WorkOrder_Source", columnList = "SourceType,SourceId")
    })
public class WorkOrder extends OrderAggregateRoot {

  /** 产出SKU */
  @Column(name = "SkuId", nullable = false)
  private Long skuId;

  /** 计划数量 */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal quantity;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 状态：草稿、已下达、生产中、已完工、已关闭、已取消 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 来源类型：销售订单、库存补货、返工 */
  @Nationalized
  @Column(name = "SourceType", nullable = false, length = 50)
  private String sourceType;

  /** 来源单据Id（可空，库存补货时为空） */
  @Column(name = "SourceId")
  @Nullable
  private Long sourceId;

  /** 来源单号（可空，冗余存储便于追溯与展示） */
  @Nationalized
  @Column(name = "SourceNumber", length = 50)
  @Nullable
  private String sourceNumber;

  /** 展开所用的BOM（可空，产品没有 BOM 时为空） */
  @Column(name = "BomId")
  @Nullable
  private Long bomId;

  /** 快照所用的工艺路线（可空，下达后写入） */
  @Column(name = "RoutingId")
  @Nullable
  private Long routingId;

  /** 工艺路线版本号（可空，快照） */
  @Nationalized
  @Column(name = "RoutingVersion", length = 50)
  @Nullable
  private String routingVersion;

  /** 计划开工时间（可空） */
  @Column(name = "PlannedStartTime")
  @Nullable
  private LocalDateTime plannedStartTime;

  /** 计划完工时间（可空） */
  @Column(name = "PlannedEndTime")
  @Nullable
  private LocalDateTime plannedEndTime;

  /** 实际开工时间（可空） */
  @Column(name = "ActualStartTime")
  @Nullable
  private LocalDateTime actualStartTime;

  /** 实际完工时间（可空） */
  @Column(name = "ActualEndTime")
  @Nullable
  private LocalDateTime actualEndTime;

  /** 已完工数量 */
  @Column(name = "CompletedQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal completedQuantity = BigDecimal.ZERO;

  /** 报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal scrapQuantity = BigDecimal.ZERO;

  /** 在制仓Id（可空）：领料后未耗用的料存放处 */
  @Column(name = "WipWarehouseId")
  @Nullable
  private Long wipWarehouseId;

  /** 成品仓Id（可空）：完工入库目标 */
  @Column(name = "FgWarehouseId")
  @Nullable
  private Long fgWarehouseId;

  /** 报废仓Id（可空）：报废品存放处 */
  @Column(name = "ScrapWarehouseId")
  @Nullable
  private Long scrapWarehouseId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 工单工序（下达时按路线快照生成） */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "WorkOrderId")
  @SQLRestriction("IsDeleted = 0")
  private List<WorkOrderOperation> workOrderOperations = new ArrayList<>();

  /** 工单用料（下达时按 BOM 展开生成） */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "WorkOrderId")
  @SQLRestriction("IsDeleted = 0")
  private List<WorkOrderMaterial> workOrderMaterials = new ArrayList<>();

  /**
   * 构造器
   *
   * @param skuId 产出SKU
   * @param quantity 计划数量
   * @param unitId 单位Id
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id（可空）
   * @param sourceNumber 来源单号（可空）
   * @param bomId 指定BOM（可空）
   * @param routingId 指定工艺路线（可空）
   * @param plannedStartTime 计划开工时间（可空）
   * @param plannedEndTime 计划完工时间（可空）
   * @param wipWarehouseId 在制仓Id（可空）
   * @param fgWarehouseId 成品仓Id（可空）
   * @param scrapWarehouseId 报废仓Id（可空）
   * @param remark 备注（可空）
   * @param number 单号（传空串表示由 OrderAddedEvent 的监听器后续分配）
   */
  WorkOrder(
      Long skuId,
      BigDecimal quantity,
      Long unitId,
      String sourceType,
      @Nullable Long sourceId,
      @Nullable String sourceNumber,
      @Nullable Long bomId,
      @Nullable Long routingId,
      @Nullable LocalDateTime plannedStartTime,
      @Nullable LocalDateTime plannedEndTime,
      @Nullable Long wipWarehouseId,
      @Nullable Long fgWarehouseId,
      @Nullable Long scrapWarehouseId,
      @Nullable String remark,
      String number) {
    super(number);
    apply(
        skuId,
        quantity,
        unitId,
        sourceType,
        sourceId,
        sourceNumber,
        bomId,
        routingId,
        plannedStartTime,
        plannedEndTime,
        wipWarehouseId,
        fgWarehouseId,
        scrapWarehouseId,
        remark);
    this.status = WorkOrderStatusEnum.DRAFT.getCode();
  }

  /**
   * 流水号类型
   *
   * @return 固定返回 {@code WorkOrder}
   */
  @Override
  public String getSerialNumberType() {
    return "WorkOrder";
  }

  /**
   * 获取工单工序（只读）
   *
   * @return 不可变列表
   */
  public List<WorkOrderOperation> getOperations() {
    return Collections.unmodifiableList(workOrderOperations);
  }

  /**
   * 获取工单用料（只读）
   *
   * @return 不可变列表
   */
  public List<WorkOrderMaterial> getMaterials() {
    return Collections.unmodifiableList(workOrderMaterials);
  }

  /**
   * 修改工单（仅草稿状态可改）
   *
   * @param skuId 产出SKU
   * @param quantity 计划数量
   * @param unitId 单位Id
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id（可空）
   * @param sourceNumber 来源单号（可空）
   * @param bomId 指定BOM（可空）
   * @param routingId 指定工艺路线（可空）
   * @param plannedStartTime 计划开工时间（可空）
   * @param plannedEndTime 计划完工时间（可空）
   * @param wipWarehouseId 在制仓Id（可空）
   * @param fgWarehouseId 成品仓Id（可空）
   * @param scrapWarehouseId 报废仓Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 非草稿状态时抛出
   */
  public void modify(
      Long skuId,
      BigDecimal quantity,
      Long unitId,
      String sourceType,
      @Nullable Long sourceId,
      @Nullable String sourceNumber,
      @Nullable Long bomId,
      @Nullable Long routingId,
      @Nullable LocalDateTime plannedStartTime,
      @Nullable LocalDateTime plannedEndTime,
      @Nullable Long wipWarehouseId,
      @Nullable Long fgWarehouseId,
      @Nullable Long scrapWarehouseId,
      @Nullable String remark) {
    if (!isDraft()) {
      throw new DomainException("工单已下达，不能再修改");
    }
    apply(
        skuId,
        quantity,
        unitId,
        sourceType,
        sourceId,
        sourceNumber,
        bomId,
        routingId,
        plannedStartTime,
        plannedEndTime,
        wipWarehouseId,
        fgWarehouseId,
        scrapWarehouseId,
        remark);
  }

  /**
   * 添加工序快照（仅草稿状态可加）
   *
   * <p>投入数量直接取工单计划数量——每道工序的投入都是整单的数，工序之间的流转差额由报工体现。
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param operationName 工序名称（快照）
   * @param isOptional 是否可选工序
   * @param isOutsourced 是否外协工序
   * @param workCenterId 工作中心Id
   * @param workCenterName 工作中心名称（快照）
   * @param setupTime 标准准备工时（分钟）
   * @param runTime 标准加工工时（分钟）
   * @param queueTime 标准排队工时（分钟）
   * @param standardBatchSize 标准批量（可空）
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 非草稿状态、或工序顺序重复时抛出
   */
  public void addOperation(
      Integer sequence,
      Long operationId,
      String operationName,
      boolean isOptional,
      boolean isOutsourced,
      Long workCenterId,
      String workCenterName,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable BigDecimal standardBatchSize,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
    if (!isDraft()) {
      throw new DomainException("工单已下达，不能再加工序");
    }
    if (findOperationBySequence(sequence) != null) {
      throw new DomainException("工序顺序" + sequence + "重复!");
    }
    workOrderOperations.add(
        new WorkOrderOperation(
            sequence,
            operationId,
            operationName,
            isOptional,
            isOutsourced,
            workCenterId,
            workCenterName,
            setupTime,
            runTime,
            queueTime,
            standardBatchSize,
            reworkOperationId,
            remark,
            this.quantity));
  }

  /**
   * 添加用料行（仅草稿状态可加）
   *
   * @param bomItemId 来源BOM行Id（可空）
   * @param skuId 物料SKU
   * @param requiredQuantity 需求数量
   * @param unitId 单位Id
   * @param warehouseId 发料仓库Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 非草稿状态时抛出
   */
  public void addMaterial(
      @Nullable Long bomItemId,
      Long skuId,
      BigDecimal requiredQuantity,
      Long unitId,
      @Nullable Long warehouseId,
      @Nullable String remark) {
    if (!isDraft()) {
      throw new DomainException("工单已下达，不能再加用料");
    }
    workOrderMaterials.add(
        new WorkOrderMaterial(bomItemId, skuId, requiredQuantity, unitId, warehouseId, remark));
  }

  /**
   * 下达：把已装配的工序与用料固化，工单转入已下达
   *
   * <p>下达是幂等的边界：一旦下达，工序与用料就固化在本单内，工艺路线或 BOM 后续改版不影响本单。
   *
   * @param bomId 展开所用的BOM（可空）
   * @param routingId 快照所用的工艺路线（可空）
   * @param routingVersion 工艺路线版本号（可空）
   * @throws DomainException 非草稿状态、或没有任何工序时抛出
   */
  public void release(
      @Nullable Long bomId, @Nullable Long routingId, @Nullable String routingVersion) {
    if (!isDraft()) {
      throw new DomainException("只有草稿状态的工单才能下达");
    }
    if (workOrderOperations.isEmpty()) {
      throw new DomainException("工单没有工序，不能下达");
    }

    this.bomId = bomId;
    this.routingId = routingId;
    this.routingVersion = routingVersion;
    this.status = WorkOrderStatusEnum.RELEASED.getCode();

    // 首道工序放行
    promoteNextOperation();
  }

  /**
   * 开工
   *
   * @param startTime 实际开工时间
   * @throws DomainException 非已下达状态时抛出
   */
  public void start(LocalDateTime startTime) {
    if (!isReleased()) {
      throw new DomainException("只有已下达的工单才能开工");
    }
    this.status = WorkOrderStatusEnum.IN_PROGRESS.getCode();
    this.actualStartTime = startTime;
  }

  /**
   * 完工
   *
   * <p>正常路径由末道工序完工自动触发，这里保留手工入口用于「末道工序有报废、直接结单」的场景。
   *
   * @param endTime 实际完工时间
   * @throws DomainException 非生产中的工单时抛出
   */
  public void complete(LocalDateTime endTime) {
    if (!isInProgress()) {
      throw new DomainException("只有生产中的工单才能完工");
    }
    this.status = WorkOrderStatusEnum.COMPLETED.getCode();
    this.actualEndTime = endTime;
  }

  /**
   * 关闭
   *
   * @throws DomainException 非已完工状态时抛出
   */
  public void close() {
    if (!isCompleted()) {
      throw new DomainException("只有已完工的工单才能关闭");
    }
    this.status = WorkOrderStatusEnum.CLOSED.getCode();
  }

  /**
   * 取消
   *
   * @throws DomainException 已完工、已关闭或已取消时抛出
   */
  public void cancel() {
    if (isCompleted() || isClosed() || isCancelled()) {
      throw new DomainException("当前状态的工单不能取消");
    }
    this.status = WorkOrderStatusEnum.CANCELLED.getCode();
  }

  /**
   * 工序开工
   *
   * @param workOrderOperationId 工单工序Id
   * @param operatorId 操作人Id（可空）
   * @param startTime 实际开工时间
   * @throws DomainException 工单已结束、或工序未放行时抛出
   */
  public void startOperation(
      Long workOrderOperationId, @Nullable Long operatorId, LocalDateTime startTime) {
    ensureExecutable();
    requireOperation(workOrderOperationId).start(operatorId, startTime);
    if (isReleased()) {
      start(startTime);
    }
  }

  /**
   * 工序报工
   *
   * <p>报工明细由 {@code OperationReport} 承载，这里只做汇总累加。
   *
   * @param workOrderOperationId 工单工序Id
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 本次工时（分钟）
   * @throws DomainException 工单已结束、或报工数据非法时抛出
   */
  public void reportOperation(
      Long workOrderOperationId,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      BigDecimal workTime) {
    ensureExecutable();
    requireOperation(workOrderOperationId).report(goodQuantity, scrapQuantity, workTime);
  }

  /**
   * 工序完工，并推进下一道工序
   *
   * <p>全部工序结束后，工单自动转为已完工，完工数与报废数取末道已完工工序的累计值。
   *
   * @param workOrderOperationId 工单工序Id
   * @param endTime 实际完工时间
   * @throws DomainException 工单已结束、或工序未开工时抛出
   */
  public void completeOperation(Long workOrderOperationId, LocalDateTime endTime) {
    ensureExecutable();
    requireOperation(workOrderOperationId).complete(endTime);
    promoteNextOperation();
    refreshCompletionStatus(endTime);
  }

  /**
   * 跳过工序（仅可选工序可用）
   *
   * @param workOrderOperationId 工单工序Id
   * @param endTime 操作时间
   * @throws DomainException 工单已结束、或工序不是可选工序时抛出
   */
  public void skipOperation(Long workOrderOperationId, LocalDateTime endTime) {
    ensureExecutable();
    requireOperation(workOrderOperationId).skip();
    promoteNextOperation();
    refreshCompletionStatus(endTime);
  }

  /**
   * 领料：累加工单用料行的已领数量
   *
   * @param workOrderMaterialId 工单用料Id
   * @param quantity 本次领用数量
   * @throws DomainException 工单已结束、或用料行不存在时抛出
   */
  public void issueMaterial(Long workOrderMaterialId, BigDecimal quantity) {
    ensureExecutable();
    requireMaterial(workOrderMaterialId).issue(quantity);
  }

  /**
   * 退料：累加已退数量
   *
   * @param workOrderMaterialId 工单用料Id
   * @param quantity 本次退料数量
   * @throws DomainException 工单已结束、或用料行不存在时抛出
   */
  public void returnMaterial(Long workOrderMaterialId, BigDecimal quantity) {
    ensureExecutable();
    requireMaterial(workOrderMaterialId).returnMaterial(quantity);
  }

  /**
   * 耗用：累加用料行的已耗数量
   *
   * @param workOrderMaterialId 工单用料Id
   * @param quantity 本次耗用数量
   * @throws DomainException 工单已结束、或用料行不存在时抛出
   */
  public void consumeMaterial(Long workOrderMaterialId, BigDecimal quantity) {
    ensureExecutable();
    requireMaterial(workOrderMaterialId).consume(quantity);
  }

  /**
   * 按Id查找未删除的工序
   *
   * @param workOrderOperationId 工单工序Id
   * @return 匹配到的工序，找不到返回 {@code null}
   */
  @Nullable
  public WorkOrderOperation findOperation(Long workOrderOperationId) {
    return workOrderOperations.stream()
        .filter(o -> Objects.equals(o.getId(), workOrderOperationId) && !o.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 按顺序查找未删除的工序
   *
   * @param sequence 工序顺序
   * @return 匹配到的工序，找不到返回 {@code null}
   */
  @Nullable
  public WorkOrderOperation findOperationBySequence(Integer sequence) {
    return workOrderOperations.stream()
        .filter(o -> Objects.equals(o.getSequence(), sequence) && !o.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 按Id查找未删除的用料行
   *
   * @param workOrderMaterialId 工单用料Id
   * @return 匹配到的用料行，找不到返回 {@code null}
   */
  @Nullable
  public WorkOrderMaterial findMaterial(Long workOrderMaterialId) {
    return workOrderMaterials.stream()
        .filter(m -> Objects.equals(m.getId(), workOrderMaterialId) && !m.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 是否草稿
   *
   * @return true 表示草稿
   */
  public boolean isDraft() {
    return WorkOrderStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已下达
   *
   * @return true 表示已下达
   */
  public boolean isReleased() {
    return WorkOrderStatusEnum.RELEASED.getCode().equals(this.status);
  }

  /**
   * 是否生产中
   *
   * @return true 表示生产中
   */
  public boolean isInProgress() {
    return WorkOrderStatusEnum.IN_PROGRESS.getCode().equals(this.status);
  }

  /**
   * 是否已完工
   *
   * @return true 表示已完工
   */
  public boolean isCompleted() {
    return WorkOrderStatusEnum.COMPLETED.getCode().equals(this.status);
  }

  /**
   * 是否已关闭
   *
   * @return true 表示已关闭
   */
  public boolean isClosed() {
    return WorkOrderStatusEnum.CLOSED.getCode().equals(this.status);
  }

  /**
   * 是否已取消
   *
   * @return true 表示已取消
   */
  public boolean isCancelled() {
    return WorkOrderStatusEnum.CANCELLED.getCode().equals(this.status);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param skuId 产出SKU
   * @param quantity 计划数量
   * @param unitId 单位Id
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id（可空）
   * @param sourceNumber 来源单号（可空）
   * @param bomId 指定BOM（可空）
   * @param routingId 指定工艺路线（可空）
   * @param plannedStartTime 计划开工时间（可空）
   * @param plannedEndTime 计划完工时间（可空）
   * @param wipWarehouseId 在制仓Id（可空）
   * @param fgWarehouseId 成品仓Id（可空）
   * @param scrapWarehouseId 报废仓Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException SKU、数量、单位、来源类型非法，或计划时间倒挂时抛出
   */
  private void apply(
      Long skuId,
      BigDecimal quantity,
      Long unitId,
      String sourceType,
      @Nullable Long sourceId,
      @Nullable String sourceNumber,
      @Nullable Long bomId,
      @Nullable Long routingId,
      @Nullable LocalDateTime plannedStartTime,
      @Nullable LocalDateTime plannedEndTime,
      @Nullable Long wipWarehouseId,
      @Nullable Long fgWarehouseId,
      @Nullable Long scrapWarehouseId,
      @Nullable String remark) {
    if (skuId <= 0) {
      throw new DomainException("产出SKU不能为空");
    }
    if (quantity.signum() <= 0) {
      throw new DomainException("计划数量必须大于0");
    }
    if (unitId <= 0) {
      throw new DomainException("单位不能为空");
    }
    String trimmedSourceType = StringUtils.trimToNull(sourceType);
    if (trimmedSourceType == null) {
      throw new DomainException("来源类型不能为空");
    }
    if (EnumUtils.fromCode(WorkOrderSourceTypeEnum.class, trimmedSourceType) == null) {
      throw new DomainException("来源类型" + trimmedSourceType + "有误!");
    }
    if (plannedStartTime != null
        && plannedEndTime != null
        && plannedStartTime.isAfter(plannedEndTime)) {
      throw new DomainException("计划开工时间不能晚于计划完工时间");
    }

    this.skuId = skuId;
    this.quantity = quantity;
    this.unitId = unitId;
    this.sourceType = trimmedSourceType;
    this.sourceId = sourceId;
    this.sourceNumber = sourceNumber;
    this.bomId = bomId;
    this.routingId = routingId;
    this.plannedStartTime = plannedStartTime;
    this.plannedEndTime = plannedEndTime;
    this.wipWarehouseId = wipWarehouseId;
    this.fgWarehouseId = fgWarehouseId;
    this.scrapWarehouseId = scrapWarehouseId;
    this.remark = remark;
  }

  /**
   * 校验工单处于可执行状态
   *
   * @throws DomainException 草稿、已完工、已关闭或已取消时抛出
   */
  private void ensureExecutable() {
    if (isDraft()) {
      throw new DomainException("工单未下达，不能报工");
    }
    if (isCompleted() || isClosed() || isCancelled()) {
      throw new DomainException("工单已结束，不能再报工");
    }
  }

  /**
   * 取工序，不存在则抛异常
   *
   * @param workOrderOperationId 工单工序Id
   * @return 工单工序
   * @throws DomainException 工序不存在时抛出
   */
  private WorkOrderOperation requireOperation(Long workOrderOperationId) {
    WorkOrderOperation operation = findOperation(workOrderOperationId);
    if (operation == null) {
      throw new DomainException("工单工序不存在!");
    }
    return operation;
  }

  /**
   * 取用料行，不存在则抛异常
   *
   * @param workOrderMaterialId 工单用料Id
   * @return 工单用料
   * @throws DomainException 用料行不存在时抛出
   */
  private WorkOrderMaterial requireMaterial(Long workOrderMaterialId) {
    WorkOrderMaterial material = findMaterial(workOrderMaterialId);
    if (material == null) {
      throw new DomainException("工单用料不存在!");
    }
    return material;
  }

  /**
   * 放行下一道可开工工序
   *
   * <p>按顺序扫：已结束的跳过，遇到第一道未开始的就放行；遇到已经可开工/加工中/外协中的就停，
   * 说明它前面的工序还没完，后面的不能越过。
   */
  private void promoteNextOperation() {
    for (WorkOrderOperation operation : orderedOperations()) {
      if (operation.isFinished()) {
        continue;
      }
      if (operation.isPending()) {
        operation.ready();
      }
      return;
    }
  }

  /**
   * 按顺序排列未删除的工序
   *
   * @return 有序工序列表
   */
  private List<WorkOrderOperation> orderedOperations() {
    return workOrderOperations.stream()
        .filter(o -> !o.getIsDeleted())
        .sorted(Comparator.comparing(WorkOrderOperation::getSequence))
        .toList();
  }

  /**
   * 全部工序结束后把工单置为已完工，并回填完工数与报废数
   *
   * @param endTime 实际完工时间
   */
  private void refreshCompletionStatus(LocalDateTime endTime) {
    boolean allFinished =
        workOrderOperations.stream()
            .filter(o -> !o.getIsDeleted())
            .allMatch(WorkOrderOperation::isFinished);
    if (!allFinished) {
      return;
    }
    WorkOrderOperation last = lastCompletedOperation();
    if (last != null) {
      this.completedQuantity = last.getCompletedQuantity();
      this.scrapQuantity = last.getScrapQuantity();
    }
    this.status = WorkOrderStatusEnum.COMPLETED.getCode();
    this.actualEndTime = endTime;
  }

  /**
   * 取顺序最靠后的已完工工序
   *
   * <p>工单的完工数应当以最后一道真正干完的工序为准，跳过的可选工序不计入。
   *
   * @return 末道已完工工序，没有已完工工序时返回 {@code null}
   */
  @Nullable
  private WorkOrderOperation lastCompletedOperation() {
    return workOrderOperations.stream()
        .filter(o -> !o.getIsDeleted())
        .filter(WorkOrderOperation::isCompleted)
        .max(Comparator.comparing(WorkOrderOperation::getSequence))
        .orElse(null);
  }
}
