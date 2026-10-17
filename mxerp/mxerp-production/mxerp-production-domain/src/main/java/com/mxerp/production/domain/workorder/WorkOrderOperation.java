package com.mxerp.production.domain.workorder;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单工序
 *
 * <p>工单下达时从工艺路线「快照」生成：工序名、工作中心名都冗余存一份，之后路线改版不影响在制工单。
 * 同时承载执行数据：计划/实际时间、投入/良品/报废数量、实际工时。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkOrderOperation",
    indexes = {
      @Index(name = "IX_WorkOrderOperation_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_WorkOrderOperation_Status", columnList = "Status"),
      @Index(name = "IX_WorkOrderOperation_WorkCenterId", columnList = "WorkCenterId"),
      @Index(name = "IX_WorkOrderOperation_Sequence", columnList = "Sequence")
      // 注意：WorkOrderId + Sequence 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class WorkOrderOperation extends BaseDeleteEntity {

  /** 工单Id */
  @Column(name = "WorkOrderId")
  private Long workOrderId;

  /** 工序顺序 */
  @Column(name = "Sequence", nullable = false)
  private Integer sequence;

  /** 工序Id */
  @Column(name = "OperationId", nullable = false)
  private Long operationId;

  /** 工序名称（快照） */
  @Nationalized
  @Column(name = "OperationName", nullable = false, length = 100)
  private String operationName;

  /** 是否可选工序（快照） */
  @Column(name = "IsOptional", nullable = false)
  private Boolean isOptional;

  /** 是否外协工序（快照） */
  @Column(name = "IsOutsourced", nullable = false)
  private Boolean isOutsourced;

  /** 工作中心Id */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 工作中心名称（快照） */
  @Nationalized
  @Column(name = "WorkCenterName", nullable = false, length = 100)
  private String workCenterName;

  /** 标准准备工时（分钟，快照） */
  @Column(name = "SetupTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal setupTime;

  /** 标准加工工时（分钟，快照） */
  @Column(name = "RunTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal runTime;

  /** 标准排队工时（分钟，快照） */
  @Column(name = "QueueTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal queueTime;

  /** 标准批量（可空，快照） */
  @Column(name = "StandardBatchSize", precision = 18, scale = 2)
  @Nullable
  private BigDecimal standardBatchSize;

  /** 返工回到的工序Id（可空，快照） */
  @Column(name = "ReworkOperationId")
  @Nullable
  private Long reworkOperationId;

  /** 状态 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

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

  /** 投入数量 */
  @Column(name = "InputQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal inputQuantity;

  /** 良品数量 */
  @Column(name = "CompletedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal completedQuantity;

  /** 报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal scrapQuantity;

  /** 实际准备工时（分钟，可空，报工汇总） */
  @Column(name = "ActualSetupTime", precision = 18, scale = 2)
  @Nullable
  private BigDecimal actualSetupTime;

  /** 实际加工工时（分钟，可空，报工汇总） */
  @Column(name = "ActualRunTime", precision = 18, scale = 2)
  @Nullable
  private BigDecimal actualRunTime;

  /** 主操作人Id（可空） */
  @Column(name = "OperatorId")
  @Nullable
  private Long operatorId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器：从路线快照生成，初始状态为未开始
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
   * @param inputQuantity 投入数量
   */
  WorkOrderOperation(
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
      @Nullable String remark,
      BigDecimal inputQuantity) {
    apply(
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
        remark);
    if (inputQuantity.signum() < 0) {
      throw new DomainException("投入数量不能为负数");
    }
    this.status = WorkOrderOperationStatusEnum.PENDING.getCode();
    this.inputQuantity = inputQuantity;
    this.completedQuantity = BigDecimal.ZERO;
    this.scrapQuantity = BigDecimal.ZERO;
  }

  /**
   * 标记为可开工
   *
   * <p>首道工序在工单下达时调用，其余工序在上道完工时由工单统一推进。
   */
  void ready() {
    if (!isPending()) {
      return;
    }
    this.status = WorkOrderOperationStatusEnum.READY.getCode();
  }

  /**
   * 开工
   *
   * @param operatorId 操作人Id（可空）
   * @param startTime 实际开工时间
   * @throws DomainException 当前状态不允许开工时抛出
   */
  public void start(@Nullable Long operatorId, LocalDateTime startTime) {
    if (!isReady()) {
      throw new DomainException("工序" + this.operationName + "当前状态不允许开工");
    }
    this.status = WorkOrderOperationStatusEnum.IN_PROGRESS.getCode();
    this.actualStartTime = startTime;
    this.operatorId = operatorId;
  }

  /**
   * 报工：累加良品、报废与实际工时
   *
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 本次工时（分钟）
   * @throws DomainException 数量为负、或良品加报废超过投入数时抛出
   */
  public void report(BigDecimal goodQuantity, BigDecimal scrapQuantity, BigDecimal workTime) {
    if (!isInProgress()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能报工");
    }
    if (goodQuantity.signum() < 0 || scrapQuantity.signum() < 0 || workTime.signum() < 0) {
      throw new DomainException("报工数量与工时不能为负数");
    }
    BigDecimal total = this.completedQuantity.add(this.scrapQuantity)
        .add(goodQuantity)
        .add(scrapQuantity);
    if (total.compareTo(this.inputQuantity) > 0) {
      throw new DomainException("工序" + this.operationName + "累计报工数超过投入数");
    }
    this.completedQuantity = this.completedQuantity.add(goodQuantity);
    this.scrapQuantity = this.scrapQuantity.add(scrapQuantity);
    this.actualRunTime =
        (this.actualRunTime == null ? BigDecimal.ZERO : this.actualRunTime).add(workTime);
  }

  /**
   * 完工
   *
   * @param endTime 实际完工时间
   * @throws DomainException 未开工时抛出
   */
  void complete(LocalDateTime endTime) {
    if (!isInProgress() && !isOutsourced()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能完工");
    }
    this.status = WorkOrderOperationStatusEnum.COMPLETED.getCode();
    this.actualEndTime = endTime;
  }

  /**
   * 跳过（仅可选工序可用）
   *
   * <p>是否可选取自下达时的工序快照，不由调用方传入，避免外部传错把必经工序跳过。
   *
   * @throws DomainException 非可选工序不允许跳过时抛出
   */
  void skip() {
    if (!Boolean.TRUE.equals(this.isOptional)) {
      throw new DomainException("工序" + this.operationName + "不是可选工序，不能跳过");
    }
    this.status = WorkOrderOperationStatusEnum.SKIPPED.getCode();
  }

  /** 标记为外协中 */
  void outsource() {
    this.status = WorkOrderOperationStatusEnum.OUTSOURCED.getCode();
  }

  /**
   * 是否已结束（完工或跳过）
   *
   * @return true 表示已结束
   */
  boolean isFinished() {
    return isCompleted() || isSkipped();
  }

  /**
   * 是否未开始
   *
   * @return true 表示未开始
   */
  boolean isPending() {
    return WorkOrderOperationStatusEnum.PENDING.getCode().equals(this.status);
  }

  /**
   * 是否可开工
   *
   * @return true 表示可开工
   */
  boolean isReady() {
    return WorkOrderOperationStatusEnum.READY.getCode().equals(this.status);
  }

  /**
   * 是否加工中
   *
   * @return true 表示加工中
   */
  boolean isInProgress() {
    return WorkOrderOperationStatusEnum.IN_PROGRESS.getCode().equals(this.status);
  }

  /**
   * 是否已完工
   *
   * @return true 表示已完工
   */
  boolean isCompleted() {
    return WorkOrderOperationStatusEnum.COMPLETED.getCode().equals(this.status);
  }

  /**
   * 是否已跳过
   *
   * @return true 表示已跳过
   */
  boolean isSkipped() {
    return WorkOrderOperationStatusEnum.SKIPPED.getCode().equals(this.status);
  }

  /**
   * 是否外协中
   *
   * @return true 表示外协中
   */
  boolean isOutsourced() {
    return WorkOrderOperationStatusEnum.OUTSOURCED.getCode().equals(this.status);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 快照字段的校验与赋值（构造时调用一次）
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
   */
  private void apply(
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
    if (sequence <= 0) {
      throw new DomainException("工序顺序必须大于0");
    }
    if (operationId <= 0) {
      throw new DomainException("工序不能为空");
    }
    if (workCenterId <= 0) {
      throw new DomainException("工作中心不能为空");
    }
    String trimmedName = StringUtils.trimToNull(operationName);
    String trimmedWorkCenterName = StringUtils.trimToNull(workCenterName);
    if (trimmedName == null) {
      throw new DomainException("工序名称不能为空");
    }
    if (trimmedWorkCenterName == null) {
      throw new DomainException("工作中心名称不能为空");
    }
    validateNonNegative("准备工时", setupTime);
    validateNonNegative("加工工时", runTime);
    validateNonNegative("排队工时", queueTime);

    this.sequence = sequence;
    this.operationId = operationId;
    this.operationName = trimmedName;
    this.isOptional = isOptional;
    this.isOutsourced = isOutsourced;
    this.workCenterId = workCenterId;
    this.workCenterName = trimmedWorkCenterName;
    this.setupTime = setupTime;
    this.runTime = runTime;
    this.queueTime = queueTime;
    this.standardBatchSize = standardBatchSize;
    this.reworkOperationId = reworkOperationId;
    this.remark = remark;
  }

  /**
   * 校验工时字段非负
   *
   * <p>参数类型在 {@code @NullMarked} 下即非空契约，调用方必须保证不为 {@code null}，因此这里只判负数。
   *
   * @param field 字段中文名，用于拼装错误提示
   * @param value 待校验的数值
   * @throws DomainException 值为负数时抛出
   */
  private void validateNonNegative(String field, BigDecimal value) {
    if (value.signum() < 0) {
      throw new DomainException(field + "不能为负数");
    }
  }
}
