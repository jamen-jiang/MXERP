package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.routing.TimeFormulaEnum;
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
 * <p>工单下达时从工艺路线「快照」生成：工序名、工作中心名、工时与费率都冗余存一份，之后路线改版或调价都不影响
 * 在制工单——费率快照钉住的是下达当时生效的那张费率卡，具体是哪一张记在 {@code rateId}。
 *
 * <p>这里只保存工序自己的事实（累计良品、累计报废、累计工时、实际起止），不保存「计划投入数量」。工序可投入多少
 * 由前置工序的产出决定，是运行时派生值，由 {@code WorkOrder} 计算后传入报工校验——把它在本行存成静态字段，
 * 下达时前置工序还没开工，根本取不到值。
 *
 * <p>外协不是一个状态，而是本行 {@code isOutsourced} 快照属性加两个动作：{@code start} 即发料给供应商，
 * {@code confirmReturn} 即回厂确认；未确认回厂不允许报工与完工。
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
  @Column(name = "WorkOrderId", nullable = false)
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

  /** 标准排队工时（分钟，快照）：只影响交期，不计入成本 */
  @Column(name = "QueueTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal queueTime;

  /** 费率卡Id（可空，快照）：下达时实际取到价格的那张卡，为空表示这道工序不计加工成本 */
  @Column(name = "RateId")
  @Nullable
  private Long rateId;

  /** 准备费率（元/小时，快照） */
  @Column(name = "SetupRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal setupRate;

  /** 加工费率（元/小时，快照） */
  @Column(name = "RunRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal runRate;

  /** 计件费率（元/件，快照） */
  @Column(name = "QuantityRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantityRate;

  /** 工时取时公式（快照）：按件时加工工时是单件工时，按批时是每批工时 */
  @Nationalized
  @Column(name = "TimeFormula", nullable = false, length = 50)
  private String timeFormula;

  /** 标准批量（可空，快照）：按批取时时必填 */
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

  /** 计划开工时间（可空，排产后写入） */
  @Column(name = "PlannedStartTime")
  @Nullable
  private LocalDateTime plannedStartTime;

  /** 计划完工时间（可空，排产后写入） */
  @Column(name = "PlannedEndTime")
  @Nullable
  private LocalDateTime plannedEndTime;

  /** 实际开工时间（可空，外协工序为发出时间） */
  @Column(name = "ActualStartTime")
  @Nullable
  private LocalDateTime actualStartTime;

  /** 实际完工时间（可空） */
  @Column(name = "ActualEndTime")
  @Nullable
  private LocalDateTime actualEndTime;

  /** 外协回厂时间（可空，仅外协工序） */
  @Column(name = "ReturnedTime")
  @Nullable
  private LocalDateTime returnedTime;

  /** 累计良品数量 */
  @Column(name = "CompletedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal completedQuantity;

  /** 累计报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal scrapQuantity;

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
   * @param rateId 费率卡Id（可空，快照）
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param timeFormula 工时取时公式编码（快照）
   * @param standardBatchSize 标准批量（可空）
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
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
      @Nullable Long rateId,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
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
        rateId,
        setupRate,
        runRate,
        quantityRate,
        timeFormula,
        standardBatchSize,
        reworkOperationId,
        remark);
    this.status = WorkOrderOperationStatusEnum.PENDING.getCode();
    this.completedQuantity = BigDecimal.ZERO;
    this.scrapQuantity = BigDecimal.ZERO;
  }

  /**
   * 排产结果写回：记下这道工序占用的计划时段
   *
   * <p>只由 {@code WorkOrder.schedule} 调用，不对外暴露。排产可以反复跑，所以这里不做状态校验。
   *
   * @param startTime 计划开工时间
   * @param endTime 计划完工时间
   */
  void plan(LocalDateTime startTime, LocalDateTime endTime) {
    this.plannedStartTime = startTime;
    this.plannedEndTime = endTime;
  }

  /**
   * 本道工序占用的总时长（分钟）
   *
   * <p>准备 + 加工，不含排队——排队是「等上道做完」的时间，排产时用工序顺序体现，重复算会把工期撑大。
   *
   * @param quantity 计划数量
   * @return 总时长（分钟）
   */
  public BigDecimal estimatedMinutes(BigDecimal quantity) {
    return estimatedSetupMinutes(quantity).add(estimatedRunMinutes(quantity));
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
   * <p>上游报工被红冲时，工单把靠这道放行的工序收回，现场就不该再看见一个开不了工的入口。
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
   * 外协回厂确认
   *
   * @param time 回厂时间
   * @throws DomainException 非外协工序、或工序不在加工中时抛出
   */
  public void confirmReturn(LocalDateTime time) {
    if (!isOutsourced()) {
      throw new DomainException("工序" + this.operationName + "不是外协工序，无需回厂确认");
    }
    if (!isInProgress()) {
      throw new DomainException("工序" + this.operationName + "尚未发出，不能确认回厂");
    }
    this.returnedTime = time;
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
    if (isOutsourced() && this.returnedTime == null) {
      throw new DomainException("工序" + this.operationName + "外协未回厂，不能报工");
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
    this.actualRunTime =
        (this.actualRunTime == null ? BigDecimal.ZERO : this.actualRunTime).add(workTime);
  }

  /**
   * 红冲报工：把良品、报废与实际工时从累计值里减回去
   *
   * <p>已完工的工序被红冲时退回加工中并清掉完工时间——报工数没了，这道工序就不能再算做完；后续工序
   * 由工单统一收回放行。
   *
   * @param goodQuantity 冲回的良品数量
   * @param scrapQuantity 冲回的报废数量
   * @param workTime 冲回的工时（分钟）
   * @throws DomainException 工序未开工、数量为负、或冲回数超过工序已累计的报工数时抛出
   */
  void unreport(BigDecimal goodQuantity, BigDecimal scrapQuantity, BigDecimal workTime) {
    if (!isInProgress() && !isCompleted()) {
      throw new DomainException("工序" + this.operationName + "未开工，不能红冲报工");
    }
    if (goodQuantity.signum() < 0 || scrapQuantity.signum() < 0 || workTime.signum() < 0) {
      throw new DomainException("冲回数量与工时不能为负数");
    }
    if (this.completedQuantity.subtract(goodQuantity).signum() < 0
        || this.scrapQuantity.subtract(scrapQuantity).signum() < 0) {
      throw new DomainException("工序" + this.operationName + "冲回数超过已累计的报工数");
    }
    BigDecimal runTime = this.actualRunTime == null ? BigDecimal.ZERO : this.actualRunTime;
    if (runTime.subtract(workTime).signum() < 0) {
      throw new DomainException("工序" + this.operationName + "冲回工时超过已累计的工时");
    }
    this.completedQuantity = this.completedQuantity.subtract(goodQuantity);
    this.scrapQuantity = this.scrapQuantity.subtract(scrapQuantity);
    this.actualRunTime = runTime.subtract(workTime);
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
    if (isOutsourced() && this.returnedTime == null) {
      throw new DomainException("工序" + this.operationName + "外协未回厂，不能完工");
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
   * <p>红冲前面的报工要看后面这道工序有没有用掉料，状态说了不算——它可能刚被红冲退回加工中。
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

  /**
   * 是否外协工序（取自下达时的路线快照）
   *
   * @return true 表示外协工序
   */
  boolean isOutsourced() {
    return Boolean.TRUE.equals(this.isOutsourced);
  }

  // ==================== 估算：工时与成本 ====================

  /**
   * 估算准备工时（分钟）
   *
   * <p>按批取时意味着每批都要重新装夹、调机，所以准备工时也随批次数放大；按件取时时整单只做一次准备。
   *
   * @param quantity 计划数量
   * @return 估算准备工时（分钟）
   * @throws DomainException 数量为负、或按批取时却没有标准批量时抛出
   */
  public BigDecimal estimatedSetupMinutes(BigDecimal quantity) {
    checkQuantity(quantity);
    return this.setupTime.multiply(batchCount(quantity));
  }

  /**
   * 估算加工工时（分钟）
   *
   * <p>按件口径下 {@code runTime} 是单件工时，乘以数量；按批口径下它是每批工时，乘以批次数。
   *
   * @param quantity 计划数量
   * @return 估算加工工时（分钟）
   * @throws DomainException 数量为负、或按批取时却没有标准批量时抛出
   */
  public BigDecimal estimatedRunMinutes(BigDecimal quantity) {
    checkQuantity(quantity);
    if (isBatchFormula()) {
      return this.runTime.multiply(batchCount(quantity));
    }
    return this.runTime.multiply(quantity);
  }

  /**
   * 估算本道工序的加工成本（元，保留 4 位）
   *
   * <p>口径与 Dynamics 365 一致：三块相加——准备与加工按时间（钱 = 时间 × 费率，费率按小时维护而工时是
   * 分钟，所以要除以 60），计件按产出（钱 = 数量 × 元/件）。不需要某一块的工序把对应费率填 0 即可，
   * 所以不必再引入一个「计价方式」枚举。排队工时不计费——机器没在干活的那段时间摊不进加工成本，它只影响交期。
   * 这里刻意保留 4 位小数，是因为单道工序的金额常常不到 1 分钱，先舍入到 2 位会把整道的成本抹成 0；
   * 由 {@code WorkOrder} 汇总后再收到 2 位。
   *
   * <p>外协工序仍按快照上的内部费率估算，真正的加工费属于采购价，不在本版本里。
   *
   * @param quantity 计划数量
   * @return 估算成本（元）
   * @throws DomainException 数量为负、或按批取时却没有标准批量时抛出
   */
  public BigDecimal estimatedCost(BigDecimal quantity) {
    BigDecimal hours = BigDecimal.valueOf(60);
    BigDecimal setupCost =
        estimatedSetupMinutes(quantity)
            .multiply(this.setupRate)
            .divide(hours, 4, RoundingMode.HALF_UP);
    BigDecimal runCost =
        estimatedRunMinutes(quantity).multiply(this.runRate).divide(hours, 4, RoundingMode.HALF_UP);
    BigDecimal quantityCost = quantity.multiply(this.quantityRate);
    return setupCost.add(runCost).add(quantityCost).setScale(4, RoundingMode.HALF_UP);
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
   * @param rateId 费率卡Id（可空，快照）
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param timeFormula 工时取时公式编码（快照）
   * @param standardBatchSize 标准批量（可空）
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 快照字段非法时抛出
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
      @Nullable Long rateId,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      String timeFormula,
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
    // 费率卡可空（为空即这道工序不计成本），一旦填写必须是有效Id
    if (rateId != null && rateId <= 0) {
      throw new DomainException("费率卡不能为空");
    }
    // 卡与价是一对：没挂卡却快照出费率，说明下达装配时把别处的价格串到了这一行
    if (rateId == null
        && (setupRate.signum() > 0 || runRate.signum() > 0 || quantityRate.signum() > 0)) {
      throw new DomainException("工序" + trimmedName + "没有费率卡，不能快照出费率");
    }
    validateNonNegative("准备工时", setupTime);
    validateNonNegative("加工工时", runTime);
    validateNonNegative("排队工时", queueTime);
    validateNonNegative("准备费率", setupRate);
    validateNonNegative("加工费率", runRate);
    validateNonNegative("计件费率", quantityRate);
    String trimmedFormula = StringUtils.trimToNull(timeFormula);
    if (trimmedFormula == null) {
      throw new DomainException("工时取时公式不能为空");
    }
    if (EnumUtils.fromCode(TimeFormulaEnum.class, trimmedFormula) == null) {
      throw new DomainException("工时取时公式" + trimmedFormula + "有误!");
    }
    // 取时公式与批量是一对：按批取时缺了批量，这道工序的工时与成本都算不出来
    if (TimeFormulaEnum.BATCH.getCode().equals(trimmedFormula) && standardBatchSize == null) {
      throw new DomainException("工序" + trimmedName + "按批计算工时，但快照里没有标准批量");
    }

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
    this.rateId = rateId;
    this.setupRate = setupRate;
    this.runRate = runRate;
    this.quantityRate = quantityRate;
    this.timeFormula = trimmedFormula;
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

  /**
   * 本道工序是否按批取时（取自路线快照）
   *
   * @return true 表示按批
   */
  private boolean isBatchFormula() {
    return TimeFormulaEnum.BATCH.getCode().equals(this.timeFormula);
  }

  /**
   * 批次数：按批取时时为 ⌈数量 ÷ 标准批量⌉，不足一批按一批算；按件取时恒为 1
   *
   * @param quantity 计划数量
   * @return 批次数
   * @throws DomainException 按批取时却没维护标准批量时抛出
   */
  private BigDecimal batchCount(BigDecimal quantity) {
    if (!isBatchFormula()) {
      return BigDecimal.ONE;
    }
    if (this.standardBatchSize == null || this.standardBatchSize.signum() <= 0) {
      throw new DomainException("工序" + this.operationName + "按批计算工时，但未维护标准批量");
    }
    return quantity.divide(this.standardBatchSize, 0, RoundingMode.CEILING);
  }
}
