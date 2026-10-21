package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
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
 * 工艺路线工序
 *
 * <p>描述「这道工序在这个物料上怎么做」：顺序、工作中心、工时、取时公式、批量与费率卡引用。工序本身是什么由
 * {@code Operation} 定义，这里只引用它的 Id。价格不在这一行，只留一个 {@code rateId} 指向
 * {@code OperationRate}——调价不该逼着路线升版，所以把钱单独做成带生效期的卡。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "RoutingOperation",
    indexes = {
      @Index(name = "IX_RoutingOperation_RoutingId", columnList = "RoutingId"),
      @Index(name = "IX_RoutingOperation_OperationId", columnList = "OperationId"),
      @Index(name = "IX_RoutingOperation_WorkCenterId", columnList = "WorkCenterId"),
      @Index(name = "IX_RoutingOperation_Sequence", columnList = "Sequence")
      // 注意：RoutingId + Sequence 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class RoutingOperation extends BaseDeleteEntity {

  /** 工艺路线Id */
  @Column(name = "RoutingId", nullable = false)
  private Long routingId;

  /** 工序顺序 */
  @Column(name = "Sequence", nullable = false)
  private Integer sequence;

  /** 工序Id */
  @Column(name = "OperationId", nullable = false)
  private Long operationId;

  /** 工作中心Id */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 准备工时（分钟） */
  @Column(name = "SetupTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal setupTime;

  /** 加工工时（分钟） */
  @Column(name = "RunTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal runTime;

  /** 排队工时（分钟） */
  @Column(name = "QueueTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal queueTime;

  /** 费率卡Id（可空，指向 OperationRate 系列）：为空表示这道工序不计加工成本 */
  @Column(name = "RateId")
  @Nullable
  private Long rateId;

  /** 工时取时公式：按件时加工工时是单件工时，按批时是每批工时 */
  @Nationalized
  @Column(name = "TimeFormula", nullable = false, length = 50)
  private String timeFormula;

  /** 标准批量（可空）：按批取时时必填，表示一批能同时处理多少 */
  @Column(name = "StandardBatchSize", precision = 18, scale = 2)
  @Nullable
  private BigDecimal standardBatchSize;

  /** 是否外协：外协工序不占本厂产能 */
  @Column(name = "IsOutsourced", nullable = false)
  private Boolean isOutsourced;

  /** 是否可选工序：非每单必经（染色、防蓝光、割边） */
  @Column(name = "IsOptional", nullable = false)
  private Boolean isOptional;

  /** 返工回到的工序Id（可空）：为空表示返工回到本工序 */
  @Column(name = "ReworkOperationId")
  @Nullable
  private Long reworkOperationId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param rateId 费率卡Id（可空，为空表示不计加工成本）
   * @param timeFormula 工时取时公式编码
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   */
  RoutingOperation(
      int sequence,
      long operationId,
      long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      boolean isOutsourced,
      boolean isOptional,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
    apply(
        sequence,
        operationId,
        workCenterId,
        setupTime,
        runTime,
        queueTime,
        rateId,
        timeFormula,
        standardBatchSize,
        isOutsourced,
        isOptional,
        reworkOperationId,
        remark);
  }

  /**
   * 修改路线工序
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param rateId 费率卡Id（可空，为空表示不计加工成本）
   * @param timeFormula 工时取时公式编码
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   */
  void modify(
      int sequence,
      long operationId,
      long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      boolean isOutsourced,
      boolean isOptional,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
    apply(
        sequence,
        operationId,
        workCenterId,
        setupTime,
        runTime,
        queueTime,
        rateId,
        timeFormula,
        standardBatchSize,
        isOutsourced,
        isOptional,
        reworkOperationId,
        remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「主键/顺序校验 → 工时非负校验 → 取时公式与批量校验 → 返工校验 → 赋值」收敛到一处，
   * 避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param rateId 费率卡Id（可空，为空表示不计加工成本）
   * @param timeFormula 工时取时公式编码
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 顺序、工序、工作中心、费率卡、工时、取时公式、批量、返工工序非法时抛出
   */
  private void apply(
      int sequence,
      long operationId,
      long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      boolean isOutsourced,
      boolean isOptional,
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
    // 费率卡可空（为空即这道工序不计成本），一旦填写必须是有效Id
    if (rateId != null && rateId <= 0) {
      throw new DomainException("费率卡不能为空");
    }
    validateNonNegative("准备工时", setupTime);
    validateNonNegative("加工工时", runTime);
    validateNonNegative("排队工时", queueTime);
    String trimmedFormula = StringUtils.trimToNull(timeFormula);
    if (trimmedFormula == null) {
      throw new DomainException("工时取时公式不能为空");
    }
    if (EnumUtils.fromCode(TimeFormulaEnum.class, trimmedFormula) == null) {
      throw new DomainException("工时取时公式" + trimmedFormula + "有误!");
    }
    // 标准批量可空（按件取时不需要），一旦填写必须大于 0
    if (standardBatchSize != null && standardBatchSize.signum() <= 0) {
      throw new DomainException("标准批量必须大于0");
    }
    // 按批取时时，批次数要靠标准批量算，缺了它整道工序的工时无法计算
    if (TimeFormulaEnum.BATCH.getCode().equals(trimmedFormula) && standardBatchSize == null) {
      throw new DomainException("工序按批计算工时，必须维护标准批量");
    }
    // 返工工序可空（为空表示返工回本工序），一旦填写必须是有效Id
    if (reworkOperationId != null && reworkOperationId <= 0) {
      throw new DomainException("返工工序不能为空");
    }

    this.sequence = sequence;
    this.operationId = operationId;
    this.workCenterId = workCenterId;
    this.setupTime = setupTime;
    this.runTime = runTime;
    this.queueTime = queueTime;
    this.rateId = rateId;
    this.timeFormula = trimmedFormula;
    this.standardBatchSize = standardBatchSize;
    this.isOutsourced = isOutsourced;
    this.isOptional = isOptional;
    this.reworkOperationId = reworkOperationId;
    this.remark = remark;
  }

  /**
   * 校验数值字段非负
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
