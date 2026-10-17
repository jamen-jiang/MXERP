package com.mxerp.production.domain.workcenter;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
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

/** 工作中心 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkCenter",
    indexes = {
      @Index(name = "IX_WorkCenter_Code", columnList = "Code"),
      @Index(name = "IX_WorkCenter_Name", columnList = "Name"),
      @Index(name = "IX_WorkCenter_Type", columnList = "Type"),
      @Index(name = "IX_WorkCenter_Disabled", columnList = "Disabled")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class WorkCenter extends UpdateAggregateRoot {

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 类型：设备、产线、班组 */
  @Nationalized
  @Column(name = "Type", nullable = false, length = 50)
  private String type;

  /** 日产能（工时或件数） */
  @Column(name = "CapacityPerDay", nullable = false, precision = 18, scale = 4)
  private BigDecimal capacityPerDay;

  /** 费率（元/小时） */
  @Column(name = "CostRate", nullable = false, precision = 18, scale = 4)
  private BigDecimal costRate;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  WorkCenter(
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, type, capacityPerDay, costRate, disabled, remark);
  }

  /**
   * 修改工作中心
   *
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, type, capacityPerDay, costRate, disabled, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 枚举校验 → 数值非负校验 → 赋值」收敛到一处，避免构造器和
   * {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  private void apply(
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    String trimmedType = StringUtils.trimToNull(type);
    if (trimmedCode == null) {
      throw new DomainException("工作中心代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工作中心名称不能为空");
    }
    if (trimmedType == null) {
      throw new DomainException("工作中心类型不能为空");
    }
    if (EnumUtils.fromCode(WorkCenterTypeEnum.class, trimmedType) == null) {
      throw new DomainException("工作中心类型" + trimmedType + "有误!");
    }
    validateNonNegative("日产能", capacityPerDay);
    validateNonNegative("费率", costRate);

    this.code = trimmedCode;
    this.name = trimmedName;
    this.type = trimmedType;
    this.capacityPerDay = capacityPerDay;
    this.costRate = costRate;
    this.disabled = disabled;
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
