package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
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
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工艺路线工序
 *
 * <p>描述「这道工序在这个产品上怎么做」：顺序、工作中心、工时、批量。工序本身是什么由
 * {@code Operation} 定义，这里只引用它的 Id。
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
  @Column(name = "RoutingId")
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

  /** 标准批量（可空）：按批生产的工序使用，如镀膜一炉装几片 */
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
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   */
  RoutingOperation(
      Integer sequence,
      Long operationId,
      Long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
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
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   */
  void modify(
      Integer sequence,
      Long operationId,
      Long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
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
   * <p>把「主键/顺序校验 → 工时非负校验 → 批量与返工校验 → 赋值」收敛到一处，避免构造器和
   * {@link #modify} 各写一遍导致规则漂移。
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      Integer sequence,
      Long operationId,
      Long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
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
    validateNonNegative("准备工时", setupTime);
    validateNonNegative("加工工时", runTime);
    validateNonNegative("排队工时", queueTime);
    // 标准批量可空（按件生产时不需要），一旦填写必须大于 0
    if (standardBatchSize != null && standardBatchSize.signum() <= 0) {
      throw new DomainException("标准批量必须大于0");
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
    this.standardBatchSize = standardBatchSize;
    this.isOutsourced = isOutsourced;
    this.isOptional = isOptional;
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
