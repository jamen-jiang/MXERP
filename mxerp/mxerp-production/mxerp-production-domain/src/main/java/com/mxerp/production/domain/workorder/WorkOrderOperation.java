package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.routing.TimeModeEnum;
import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.math.RoundingMode;
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
 * <p>工单下达时从工艺路线「快照」生成：工序名、工作中心名与工时都冗余存一份，之后路线改版不影响在制工单。
 *
 * <p>这里只保存工序自己的事实（累计良品、累计报废、累计工时、实际起止），不保存「计划投入数量」。工序可投入多少
 * 由前置工序的产出决定，是运行时派生值，由 {@code WorkOrder} 计算后传入报工校验——把它在本行存成静态字段，
 * 下达时前置工序还没开工，根本取不到值。
 *
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

  /** 工作中心Id */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 工作中心名称（快照） */
  @Nationalized
  @Column(name = "WorkCenterName", nullable = false, length = 100)
  private String workCenterName;

  /** 单次循环工时（分钟，快照） */
  @Column(name = "CycleTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal cycleTime;

  /** 工时计算方式（快照）：固定=手工工时；按历史计算=取最近若干张工单平均 */
  @Nationalized
  @Column(name = "TimeMode", nullable = false, length = 50)
  private String timeMode;

  /** 状态 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 实际开工时间（可空，外协工序为发出时间） */
  @Column(name = "ActualStartTime")
  @Nullable
  private LocalDateTime actualStartTime;

  /** 实际完工时间（可空） */
  @Column(name = "ActualEndTime")
  @Nullable
  private LocalDateTime actualEndTime;

  /** 累计良品数量 */
  @Column(name = "CompletedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal completedQuantity;

  /** 累计报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal scrapQuantity;

  /** 实际加工工时（分钟，可空，报工汇总） */
  @Column(name = "ActualCycleTime", precision = 18, scale = 2)
  @Nullable
  private BigDecimal actualCycleTime;

  /** 主操作人Id（可空） */
  @Column(name = "OperatorId")
  @Nullable
  private Long operatorId;

  /**
   * 构造器：从路线快照生成，初始状态为未开始
   *
   * @param operationId 工序Id
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id
   * @param workCenterName 工作中心名称（快照）
   * @param timeMode 工时计算方式编码（快照）
   * @param cycleTime 单次循环工时（分钟）
   * @param sequence 工序顺序
   */
  WorkOrderOperation(
      long operationId,
      String operationName,
      long workCenterId,
      String workCenterName,
      String timeMode,
      BigDecimal cycleTime,
      int sequence) {
    apply(
        operationId,
        operationName,
        workCenterId,
        workCenterName,
        timeMode,
        cycleTime,
        sequence);
    this.status = WorkOrderOperationStatusEnum.PENDING.getCode();
    this.completedQuantity = BigDecimal.ZERO;
    this.scrapQuantity = BigDecimal.ZERO;
  }

  /**
   * 本道工序占用的总时长（分钟）
   *
   * <p>准备 + 加工，不含排队——排队是「等上道做完」的时间，用工序顺序体现，重复算会把工期撑大。
   *
   * @param quantity 计划数量
   * @return 总时长（分钟）
   */
  public BigDecimal estimatedMinutes(BigDecimal quantity) {
    return estimatedRunMinutes(quantity);
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
   * 收回放行（可开工 → 未开始）
   *
   * <p>上游报工被撤回时，工单把靠这道放行的工序收回，现场就不该再看见一个开不了工的入口。
   */
  void hold() {
    if (!isReady()) {
      return;
    }
    this.status = WorkOrderOperationStatusEnum.PENDING.getCode();
  }

  /**
   * 开工（外协工序即发料给供应商）
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
   * <p>上限由调用方（工单聚合）算出的可用数量决定：本工序最多能报出前置工序流转过来的数量。
   *
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param workTime 本次工时（分钟）
   * @param availableQuantity 本工序可投入数量
   * @throws DomainException 工序状态不允许、数量为负、或累计报工数超过可投入数时抛出
   */
  public void report(
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      BigDecimal workTime,
      BigDecimal availableQuantity) {
    if (!isInProgress()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能报工");
    }
    if (goodQuantity.signum() < 0 || scrapQuantity.signum() < 0 || workTime.signum() < 0) {
      throw new DomainException("报工数量与工时不能为负数");
    }
    BigDecimal total =
        this.completedQuantity.add(this.scrapQuantity).add(goodQuantity).add(scrapQuantity);
    if (total.compareTo(availableQuantity) > 0) {
      throw new DomainException(
          "工序" + this.operationName + "累计报工数" + total + "超过可投入数量" + availableQuantity);
    }
    this.completedQuantity = this.completedQuantity.add(goodQuantity);
    this.scrapQuantity = this.scrapQuantity.add(scrapQuantity);
    this.actualCycleTime =
        (this.actualCycleTime == null ? BigDecimal.ZERO : this.actualCycleTime).add(workTime);
  }

  /**
   * 撤回报工：把良品、报废与实际工时从累计值里减回去
   *
   * <p>已完工的工序被撤回时退回加工中并清掉完工时间——报工数没了，这道工序就不能再算做完；后续工序
   * 由工单统一收回放行。
   *
   * @param goodQuantity 冲回的良品数量
   * @param scrapQuantity 冲回的报废数量
   * @param workTime 冲回的工时（分钟）
   * @throws DomainException 工序未开工、数量为负、或冲回数超过工序已累计的报工数时抛出
   */
  void unreport(BigDecimal goodQuantity, BigDecimal scrapQuantity, BigDecimal workTime) {
    if (!isInProgress() && !isCompleted()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能撤回报工");
    }
    if (goodQuantity.signum() < 0 || scrapQuantity.signum() < 0 || workTime.signum() < 0) {
      throw new DomainException("冲回数量与工时不能为负数");
    }
    if (this.completedQuantity.subtract(goodQuantity).signum() < 0
        || this.scrapQuantity.subtract(scrapQuantity).signum() < 0) {
      throw new DomainException("工序" + this.operationName + "冲回数超过已累计的报工数");
    }
    BigDecimal cycleTime = this.actualCycleTime == null ? BigDecimal.ZERO : this.actualCycleTime;
    if (cycleTime.subtract(workTime).signum() < 0) {
      throw new DomainException("工序" + this.operationName + "冲回工时超过已累计的工时");
    }
    this.completedQuantity = this.completedQuantity.subtract(goodQuantity);
    this.scrapQuantity = this.scrapQuantity.subtract(scrapQuantity);
    this.actualCycleTime = cycleTime.subtract(workTime);
    if (isCompleted()) {
      this.status = WorkOrderOperationStatusEnum.IN_PROGRESS.getCode();
      this.actualEndTime = null;
    }
  }

  /**
   * 完工
   *
   * @param endTime 实际完工时间
   * @throws DomainException 未开工、或外协工序未回厂时抛出
   */
  void complete(LocalDateTime endTime) {
    if (!isInProgress()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能完工");
    }
    this.status = WorkOrderOperationStatusEnum.COMPLETED.getCode();
    this.actualEndTime = endTime;
  }

  /**
   * 跳过
   *
   * <p>把这道工序标记为不执行，后续工序照常推进。
   */
  void skip() {
    this.status = WorkOrderOperationStatusEnum.SKIPPED.getCode();
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
   * 是否已经报过工（累计良品加报废不为零）
   *
   * <p>撤回前面的报工要看后面这道工序有没有用掉料，状态说了不算——它可能刚被撤回退回加工中。
   *
   * @return true 表示累计报工数不为零
   */
  boolean hasReported() {
    return this.completedQuantity.add(this.scrapQuantity).signum() != 0;
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
   * 是否加工中（外协工序表示已发出待回厂）
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

  // ==================== 估算：工时与成本 ====================

  /**
   * 估算单次循环工时（分钟）
   *
   * <p>手工模式下 {@code cycleTime} 是固定工时，乘以数量；自动模式应取历史工单平均，
   * 但历史取数需要仓储支持，本版本按固定工时兜底。
   *
   * @param quantity 计划数量
   * @return 估算单次循环工时（分钟）
   * @throws DomainException 数量为负时抛出
   */
  public BigDecimal estimatedRunMinutes(BigDecimal quantity) {
    checkQuantity(quantity);
    return this.cycleTime.multiply(quantity);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 快照字段的校验与赋值（构造时调用一次）
   *
   * @param operationId 工序Id
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id
   * @param workCenterName 工作中心名称（快照）
   * @param timeMode 工时计算方式编码（快照）
   * @param cycleTime 单次循环工时（分钟）
   * @param sequence 工序顺序
   * @throws DomainException 快照字段非法时抛出
   */
  private void apply(
      long operationId,
      String operationName,
      long workCenterId,
      String workCenterName,
      String timeMode,
      BigDecimal cycleTime,
      int sequence) {
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
    validateNonNegative("循环工时", cycleTime);
    String trimmedMode = StringUtils.trimToNull(timeMode);
    if (trimmedMode == null) {
      throw new DomainException("工时计算方式不能为空");
    }
    if (EnumUtils.fromCode(TimeModeEnum.class, trimmedMode) == null) {
      throw new DomainException("工时计算方式" + trimmedMode + "有误!");
    }

    this.operationId = operationId;
    this.operationName = trimmedName;
    this.workCenterId = workCenterId;
    this.workCenterName = trimmedWorkCenterName;
    this.timeMode = trimmedMode;
    this.cycleTime = cycleTime;
    this.sequence = sequence;
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

  /**
   * 校验参与估算的数量
   *
   * @param quantity 计划数量
   * @throws DomainException 数量为负时抛出
   */
  private void checkQuantity(BigDecimal quantity) {
    if (quantity.signum() < 0) {
      throw new DomainException("计划数量不能为负数");
    }
  }
}
