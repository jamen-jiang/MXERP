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

/**
 * 工艺路线工序
 *
 * <p>描述「这道工序在这个物料上怎么做」：顺序、工作中心、循环工时与计算方式。工序本身是什么由 {@code Operation} 定义，这里只引用它的 Id。
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

  /** 工序Id */
  @Column(name = "OperationId", nullable = false)
  private Long operationId;

  /** 工作中心Id */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 工时计算方式：固定=用手工填的工时；按历史计算=取最近若干张工单的平均工时 */
  @Nationalized
  @Column(name = "TimeMode", nullable = false, length = 50)
  private String timeMode;

  /** 单次循环工时（分钟，照 Odoo 的 time_cycle_manual）：一次加工循环的时长 */
  @Column(name = "CycleTime", nullable = false, precision = 18, scale = 2)
  private BigDecimal cycleTime;

  /** 工序顺序 */
  @Column(name = "Sequence", nullable = false)
  private Integer sequence;

  /**
   * 构造器
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param timeMode 工时计算方式编码
   * @param cycleTime 单次循环工时（分钟）
   * @param sequence 工序顺序
   */
  RoutingOperation(
      long operationId, long workCenterId, String timeMode, BigDecimal cycleTime, int sequence) {
    apply(operationId, workCenterId, timeMode, cycleTime, sequence);
  }

  /**
   * 修改路线工序
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param timeMode 工时计算方式编码
   * @param cycleTime 单次循环工时（分钟）
   * @param sequence 工序顺序
   */
  void modify(
      long operationId, long workCenterId, String timeMode, BigDecimal cycleTime, int sequence) {
    apply(operationId, workCenterId, timeMode, cycleTime, sequence);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「主键/顺序校验 → 工时非负校验 → 计算方式校验 → 赋值」收敛到一处， 避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param timeMode 工时计算方式编码
   * @param cycleTime 单次循环工时（分钟）
   * @param sequence 工序顺序
   * @throws DomainException 工序、工作中心、计算方式、工时或顺序非法时抛出
   */
  private void apply(
      long operationId, long workCenterId, String timeMode, BigDecimal cycleTime, int sequence) {
    if (sequence <= 0) {
      throw new DomainException("工序顺序必须大于0");
    }
    if (operationId <= 0) {
      throw new DomainException("工序不能为空");
    }
    if (workCenterId <= 0) {
      throw new DomainException("工作中心不能为空");
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
    this.workCenterId = workCenterId;
    this.timeMode = trimmedMode;
    this.cycleTime = cycleTime;
    this.sequence = sequence;
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
