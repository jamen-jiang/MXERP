package com.mxerp.production.domain.operation;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
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
 * 工序（工艺字典）
 *
 * <p>只描述「这道工序本身是什么」，不描述它在某个物料上做多久——那是「在一条具体路线里怎么用这道工序」的
 * 决策，属于 {@code RoutingOperation}。
 *
 * <p>{@code workCenterId} 是默认工作中心：建路线时预填，路线里可以改。同一道「抛光」在 A 物料走 1 号机、
 * 在 B 物料走 2 号机，默认值省得每条路线都选一遍。
 *
 * <p>字段刻意保持精简：工序表一旦膨胀，工艺员维护不动，最后一定退化成没人看的垃圾字段。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Operation",
    indexes = {
      @Index(name = "IX_Operation_Code", columnList = "Code"),
      @Index(name = "IX_Operation_Name", columnList = "Name"),
      @Index(name = "IX_Operation_WorkCenterId", columnList = "WorkCenterId"),
      @Index(name = "IX_Operation_Disabled", columnList = "Disabled")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Operation extends UpdateAggregateRoot {

  /** 工序代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 工序名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 默认工作中心Id（可空，建路线时预填，路线里可改） */
  @Column(name = "WorkCenterId")
  @Nullable
  private Long workCenterId;

  /** 默认批量（可空）：一次能同时处理多少件，按批取时的路线工序没填时用它 */
  @Column(name = "BatchSize", precision = 18, scale = 2)
  @Nullable
  private BigDecimal batchSize;

  /** 工序说明（可空） */
  @Nationalized
  @Column(name = "Description", length = 500)
  @Nullable
  private String description;

  /** 是否禁用：禁用后建路线时不可选，已引用的路线不受影响 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /**
   * 构造器
   *
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   */
  Operation(
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    apply(code, name, workCenterId, batchSize, description, disabled);
  }

  /**
   * 修改工序
   *
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   */
  void modify(
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    apply(code, name, workCenterId, batchSize, description, disabled);
  }

  /**
   * 启用
   *
   * <p>启用后建路线时该工序可选。
   */
  public void enable() {
    this.disabled = false;
  }

  /**
   * 禁用
   *
   * <p>禁用只影响新建路线时的可选项，已引用该工序的路线不受影响。
   */
  public void disable() {
    this.disabled = true;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 数值校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @throws DomainException 代码或名称为空、工作中心Id非法、批量非正时抛出
   */
  private void apply(
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("工序代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工序名称不能为空");
    }
    if (workCenterId != null && workCenterId <= 0) {
      throw new DomainException("默认工作中心不能为空");
    }
    if (batchSize != null && batchSize.signum() <= 0) {
      throw new DomainException("默认批量必须大于0");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.workCenterId = workCenterId;
    this.batchSize = batchSize;
    this.description = description;
    this.disabled = disabled;
  }
}
