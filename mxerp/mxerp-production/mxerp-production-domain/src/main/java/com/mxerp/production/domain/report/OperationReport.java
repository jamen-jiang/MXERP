package com.mxerp.production.domain.report;

import com.mxerp.shared.domain.aggregate.OrderAggregateRoot;
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
 * 工序报工单
 *
 * <p>为什么独立成聚合而不是给工单工序加几个字段：报工是「一次事件」，天然带操作人、起止时间、
 * 本次良品/报废。拍平成一个累计值之后，计件工资、人工成本、异常停机分析全部无从追溯。
 *
 * <p>生命周期只有两态：{@code DRAFT → SUBMITTED}。提交后不可改，改只能作废重开——这样工单工序上的
 * 累计值永远等于「所有已提交报工单之和」，不会因为补录或修改而对不上。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "OperationReport",
    indexes = {
      @Index(name = "IX_OperationReport_Number", columnList = "Number"),
      @Index(name = "IX_OperationReport_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_OperationReport_Operation", columnList = "WorkOrderOperationId"),
      @Index(name = "IX_OperationReport_Operator", columnList = "OperatorId"),
      @Index(name = "IX_OperationReport_Status", columnList = "Status")
    })
public class OperationReport extends OrderAggregateRoot {

  /** 工单Id */
  @Column(name = "WorkOrderId", nullable = false)
  private Long workOrderId;

  /** 工单号（冗余，便于展示） */
  @Nationalized
  @Column(name = "WorkOrderNumber", nullable = false, length = 30)
  private String workOrderNumber;

  /** 工单工序Id */
  @Column(name = "WorkOrderOperationId", nullable = false)
  private Long workOrderOperationId;

  /** 工序Id（快照） */
  @Column(name = "OperationId", nullable = false)
  private Long operationId;

  /** 工序名称（快照） */
  @Nationalized
  @Column(name = "OperationName", nullable = false, length = 100)
  private String operationName;

  /** 工作中心Id（快照） */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 工作中心名称（快照） */
  @Nationalized
  @Column(name = "WorkCenterName", nullable = false, length = 100)
  private String workCenterName;

  /** 操作人Id（可空） */
  @Column(name = "OperatorId")
  @Nullable
  private Long operatorId;

  /** 开始时间（可空，仅作记录） */
  @Column(name = "StartTime")
  @Nullable
  private LocalDateTime startTime;

  /** 结束时间（可空，仅作记录） */
  @Column(name = "EndTime")
  @Nullable
  private LocalDateTime endTime;

  /** 本次工时（分钟） */
  @Column(name = "WorkTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal workTime;

  /** 本次良品数量 */
  @Column(name = "GoodQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal goodQuantity;

  /** 本次报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal scrapQuantity;

  /** 状态：草稿、已提交 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param workOrderId 工单Id
   * @param workOrderNumber 工单号
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param startTime 开始时间（可空）
   * @param endTime 结束时间（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @param number 单号（传空串表示由 OrderAddedEvent 的监听器后续分配）
   */
  OperationReport(
      Long workOrderId,
      String workOrderNumber,
      Long workOrderOperationId,
      Long operationId,
      String operationName,
      Long workCenterId,
      String workCenterName,
      @Nullable Long operatorId,
      @Nullable LocalDateTime startTime,
      @Nullable LocalDateTime endTime,
      BigDecimal workTime,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable String remark,
      String number) {
    super(number);
    apply(
        workOrderId,
        workOrderNumber,
        workOrderOperationId,
        operationId,
        operationName,
        workCenterId,
        workCenterName,
        operatorId,
        startTime,
        endTime,
        workTime,
        goodQuantity,
        scrapQuantity,
        remark);
    this.status = OperationReportStatusEnum.DRAFT.getCode();
  }

  /**
   * 流水号类型
   *
   * @return 固定返回 {@code OperationReport}
   */
  @Override
  public String getSerialNumberType() {
    return "OperationReport";
  }

  /**
   * 修改报工单（仅草稿可改）
   *
   * @param workOrderId 工单Id
   * @param workOrderNumber 工单号
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param startTime 开始时间（可空）
   * @param endTime 结束时间（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @throws DomainException 已提交时抛出
   */
  public void modify(
      Long workOrderId,
      String workOrderNumber,
      Long workOrderOperationId,
      Long operationId,
      String operationName,
      Long workCenterId,
      String workCenterName,
      @Nullable Long operatorId,
      @Nullable LocalDateTime startTime,
      @Nullable LocalDateTime endTime,
      BigDecimal workTime,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable String remark) {
    if (isSubmitted()) {
      throw new DomainException("报工单已提交，不能再修改");
    }
    apply(
        workOrderId,
        workOrderNumber,
        workOrderOperationId,
        operationId,
        operationName,
        workCenterId,
        workCenterName,
        operatorId,
        startTime,
        endTime,
        workTime,
        goodQuantity,
        scrapQuantity,
        remark);
  }

  /**
   * 提交：状态置为已提交
   *
   * <p>只负责自己这一半；把它累加进工单工序汇总由 {@code OperationReportDomainService} 编排。
   *
   * @throws DomainException 已提交时抛出
   */
  public void submit() {
    if (isSubmitted()) {
      throw new DomainException("报工单已提交，不能重复提交");
    }
    this.status = OperationReportStatusEnum.SUBMITTED.getCode();
  }

  /**
   * 是否草稿
   *
   * @return true 表示草稿
   */
  public boolean isDraft() {
    return OperationReportStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已提交
   *
   * @return true 表示已提交
   */
  public boolean isSubmitted() {
    return OperationReportStatusEnum.SUBMITTED.getCode().equals(this.status);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param workOrderId 工单Id
   * @param workOrderNumber 工单号
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param startTime 开始时间（可空）
   * @param endTime 结束时间（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @throws DomainException 工单/工序为空、工时或数量为负、起止时间倒挂时抛出
   */
  private void apply(
      Long workOrderId,
      String workOrderNumber,
      Long workOrderOperationId,
      Long operationId,
      String operationName,
      Long workCenterId,
      String workCenterName,
      @Nullable Long operatorId,
      @Nullable LocalDateTime startTime,
      @Nullable LocalDateTime endTime,
      BigDecimal workTime,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable String remark) {
    if (workOrderId <= 0) {
      throw new DomainException("工单不能为空");
    }
    if (workOrderOperationId <= 0) {
      throw new DomainException("工单工序不能为空");
    }
    if (operationId <= 0) {
      throw new DomainException("工序不能为空");
    }
    if (workCenterId <= 0) {
      throw new DomainException("工作中心不能为空");
    }
    if (workTime.signum() < 0) {
      throw new DomainException("工时不能为负数");
    }
    if (goodQuantity.signum() < 0) {
      throw new DomainException("良品数量不能为负数");
    }
    if (scrapQuantity.signum() < 0) {
      throw new DomainException("报废数量不能为负数");
    }
    if (goodQuantity.signum() == 0 && scrapQuantity.signum() == 0) {
      throw new DomainException("良品数量与报废数量不能同时为0");
    }
    if (startTime != null && endTime != null && startTime.isAfter(endTime)) {
      throw new DomainException("开始时间不能晚于结束时间");
    }
    String trimmedWorkOrderNumber = StringUtils.trimToNull(workOrderNumber);
    String trimmedOperationName = StringUtils.trimToNull(operationName);
    String trimmedWorkCenterName = StringUtils.trimToNull(workCenterName);
    if (trimmedWorkOrderNumber == null) {
      throw new DomainException("工单号不能为空");
    }
    if (trimmedOperationName == null) {
      throw new DomainException("工序名称不能为空");
    }
    if (trimmedWorkCenterName == null) {
      throw new DomainException("工作中心名称不能为空");
    }

    this.workOrderId = workOrderId;
    this.workOrderNumber = trimmedWorkOrderNumber;
    this.workOrderOperationId = workOrderOperationId;
    this.operationId = operationId;
    this.operationName = trimmedOperationName;
    this.workCenterId = workCenterId;
    this.workCenterName = trimmedWorkCenterName;
    this.operatorId = operatorId;
    this.startTime = startTime;
    this.endTime = endTime;
    this.workTime = workTime;
    this.goodQuantity = goodQuantity;
    this.scrapQuantity = scrapQuantity;
    this.remark = remark;
  }
}
