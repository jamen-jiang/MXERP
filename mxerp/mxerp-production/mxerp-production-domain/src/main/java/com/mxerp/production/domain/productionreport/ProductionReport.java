package com.mxerp.production.domain.productionreport;

import com.mxerp.shared.domain.aggregate.OrderAggregateRoot;
import com.mxerp.shared.domain.enums.SerialNumberTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 生产报工单
 *
 * <p>为什么独立成聚合而不是给工单工序加几个字段：报工是「一次事件」，天然带操作人、 本次良品/报废与工时。拍平成一个累计值之后，计件工资、人工成本、异常分析全部无从追溯。
 *
 * <p>现场只该填三样：工序、良品数、报废数。工单号、工序名称、工作中心这些快照由领域服务 从工单上抄，工时不填就按工序的标准时间算——让人抄一遍就等于给自己留一个对不上的机会。
 *
 * <p>生命周期只有 {@code DRAFT ⇄ SUBMITTED}，来回都算正常操作。提交后不可改，要改就**撤回**：把这笔报工从
 * 工单工序的累计里减回去、单据退回草稿，改完再提交。这样工单工序上的累计值永远等于「当前已提交的那几张报工
 * 单之和」，不会因为补录或修改而对不上。报工不动库存，所以用不着库存单据那套「另开一张红字单、原单永不翻
 * 状态」的做法——那是给真动过货的单据留痕迹用的。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "ProductionReport",
    indexes = {
      @Index(name = "IX_ProductionReport_Number", columnList = "Number"),
      @Index(name = "IX_ProductionReport_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_ProductionReport_Operation", columnList = "WorkOrderOperationId"),
      @Index(name = "IX_ProductionReport_Operator", columnList = "OperatorId"),
      @Index(name = "IX_ProductionReport_Status", columnList = "Status")
    })
public class ProductionReport extends OrderAggregateRoot {

  /** 工单Id */
  @Column(name = "WorkOrderId", nullable = false)
  private Long workOrderId;

  /** 工单号（快照，由领域服务从工单抄） */
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

  /** 操作人Id（可空，默认沿用工序开工时记的人） */
  @Column(name = "OperatorId")
  @Nullable
  private Long operatorId;

  /** 本次工时（分钟，不填则按工序标准时间派生） */
  @Column(name = "WorkTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal workTime;

  /** 本次良品数量 */
  @Column(name = "GoodQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal goodQuantity;

  /** 本次报废数量 */
  @Column(name = "ScrapQuantity", nullable = false, precision = 18, scale = 2)
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
   * @param workOrderNumber 工单号（快照）
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @param number 单号（传空串表示由 OrderAddedEvent 的监听器后续分配）
   */
  ProductionReport(
      Long workOrderId,
      String workOrderNumber,
      Long workOrderOperationId,
      Long operationId,
      String operationName,
      Long workCenterId,
      String workCenterName,
      @Nullable Long operatorId,
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
        workTime,
        goodQuantity,
        scrapQuantity,
        remark);
    this.status = ProductionReportStatusEnum.DRAFT.getCode();
  }

  /**
   * 流水号类型
   *
   * @return 生产报工单类型
   */
  @Override
  public SerialNumberTypeEnum getSerialNumberType() {
    return SerialNumberTypeEnum.PRODUCTION_REPORT;
  }

  /**
   * 修改报工单（仅草稿可改）
   *
   * @param workOrderId 工单Id
   * @param workOrderNumber 工单号（快照）
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @throws DomainException 已提交时抛出（要改先撤回）
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
      BigDecimal workTime,
      BigDecimal goodQuantity,
      BigDecimal scrapQuantity,
      @Nullable String remark) {
    if (!isDraft()) {
      throw new DomainException("报工单不是草稿，不能再修改");
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
        workTime,
        goodQuantity,
        scrapQuantity,
        remark);
  }

  /**
   * 提交：状态置为已提交
   *
   * <p>只负责自己这一半；把它累加进工单工序汇总由 {@code ProductionReportDomainService} 编排。
   *
   * @throws DomainException 已提交时抛出
   */
  void submit() {
    if (isSubmitted()) {
      throw new DomainException("报工单已提交，不能重复提交");
    }
    this.status = ProductionReportStatusEnum.SUBMITTED.getCode();
  }

  /**
   * 撤回：状态退回草稿
   *
   * <p>只负责自己这一半；把报工数从工单工序汇总里减回去由 {@code ProductionReportDomainService} 编排，
   * 两下必须成对，否则累计值与单上之数就对不上。
   *
   * @throws DomainException 非已提交状态时抛出
   */
  void withdraw() {
    if (!isSubmitted()) {
      throw new DomainException("只有已提交的报工单才能撤回");
    }
    this.status = ProductionReportStatusEnum.DRAFT.getCode();
  }

  /**
   * 是否草稿
   *
   * @return true 表示草稿
   */
  public boolean isDraft() {
    return ProductionReportStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已提交
   *
   * @return true 表示已提交
   */
  public boolean isSubmitted() {
    return ProductionReportStatusEnum.SUBMITTED.getCode().equals(this.status);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param workOrderId 工单Id
   * @param workOrderNumber 工单号（快照）
   * @param workOrderOperationId 工单工序Id
   * @param operationId 工序Id（快照）
   * @param operationName 工序名称（快照）
   * @param workCenterId 工作中心Id（快照）
   * @param workCenterName 工作中心名称（快照）
   * @param operatorId 操作人Id（可空）
   * @param workTime 本次工时（分钟）
   * @param goodQuantity 本次良品数量
   * @param scrapQuantity 本次报废数量
   * @param remark 备注（可空）
   * @throws DomainException 工单/工序/工作中心为空、工时或数量为负、良品与报废同时为0时抛出
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
    this.workTime = workTime;
    this.goodQuantity = goodQuantity;
    this.scrapQuantity = scrapQuantity;
    this.remark = remark;
  }
}
