package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工艺路线（独立主数据，不绑定产品） */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Routing",
    indexes = {
      @Index(name = "IX_Routing_Code", columnList = "Code"),
      @Index(name = "IX_Routing_Name", columnList = "Name"),
      @Index(name = "IX_Routing_Status", columnList = "Status")
      // 注意：Code + Version 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Routing extends UpdateAggregateRoot {

  /** 路线代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 路线名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 版本号 */
  @Nationalized
  @Column(name = "Version", nullable = false, length = 50)
  private String version;

  /** 状态：草稿、已发布、已作废 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 生效时间（可空） */
  @Column(name = "EffectiveFrom")
  @Nullable
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空） */
  @Column(name = "EffectiveTo")
  @Nullable
  private LocalDateTime effectiveTo;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 路线工序列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "RoutingId")
  @SQLRestriction("IsDeleted = 0")
  private List<RoutingOperation> routingOperations = new ArrayList<>();

  /**
   * 获取路线工序列表（只读）
   *
   * @return 不可变列表
   */
  public List<RoutingOperation> getRoutingOperations() {
    return Collections.unmodifiableList(routingOperations);
  }

  /**
   * 构造器
   *
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  Routing(
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(code, name, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 修改工艺路线
   *
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(code, name, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 添加工序
   *
   * @param data 工序数据
   */
  /**
   * 添加工序
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
  public void addOperation(
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
    if (isExistSequence(sequence, null)) {
      throw new DomainException("工序顺序" + sequence + "已存在!");
    }
    routingOperations.add(
        new RoutingOperation(
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
            remark));
  }

  /**
   * 修改工序
   *
   * @param routingOperationId 路线工序Id
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
  public void modifyOperation(
      Long routingOperationId,
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
    RoutingOperation target = findOperation(routingOperationId);
    if (target == null) {
      throw new DomainException("路线工序不存在!");
    }
    if (isExistSequence(sequence, routingOperationId)) {
      throw new DomainException("工序顺序" + sequence + "已存在!");
    }
    target.modify(
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
   * 删除工序（逻辑删除）
   *
   * @param routingOperationId 路线工序Id
   */
  public void deleteOperation(Long routingOperationId) {
    RoutingOperation target = findOperation(routingOperationId);
    if (target == null) {
      return;
    }
    target.delete();
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 状态枚举校验 → 生效区间校验 → 赋值」收敛到一处，避免构造器和
   * {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    String trimmedVersion = StringUtils.trimToNull(version);
    String trimmedStatus = StringUtils.trimToNull(status);
    if (trimmedCode == null) {
      throw new DomainException("工艺路线代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工艺路线名称不能为空");
    }
    if (trimmedVersion == null) {
      throw new DomainException("版本号不能为空");
    }
    if (trimmedStatus == null) {
      throw new DomainException("状态不能为空");
    }
    if (EnumUtils.fromCode(RoutingStatusEnum.class, trimmedStatus) == null) {
      throw new DomainException("状态" + trimmedStatus + "有误!");
    }
    if (effectiveFrom != null && effectiveTo != null && effectiveFrom.isAfter(effectiveTo)) {
      throw new DomainException("生效时间不能晚于失效时间");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.version = trimmedVersion;
    this.status = trimmedStatus;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.remark = remark;
  }

  /**
   * 在路线工序列表中按 Id 查找未删除的工序
   *
   * @param routingOperationId 路线工序Id
   * @return 匹配到的工序，找不到返回 {@code null}
   */
  @Nullable
  private RoutingOperation findOperation(Long routingOperationId) {
    return routingOperations.stream()
        .filter(o -> Objects.equals(o.getId(), routingOperationId) && !o.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 判断工序顺序是否已被占用
   *
   * <p>工序顺序在单条路线内必须唯一；修改工序时需排除自身，否则会把「保持原顺序」误判为冲突。
   *
   * @param sequence 待校验的工序顺序
   * @param excludeId 需要排除的路线工序Id（新增时传 {@code null}）
   * @return true：已被其它未删除的工序占用
   */
  private boolean isExistSequence(Integer sequence, @Nullable Long excludeId) {
    return routingOperations.stream()
        .filter(o -> !o.getIsDeleted())
        .filter(o -> !Objects.equals(o.getId(), excludeId))
        .anyMatch(o -> Objects.equals(o.getSequence(), sequence));
  }
}
