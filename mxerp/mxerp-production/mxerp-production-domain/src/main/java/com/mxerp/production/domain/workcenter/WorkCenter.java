package com.mxerp.production.domain.workcenter;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.math.BigDecimal;
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

/**
 * 工作中心
 *
 * <p>排程与成本归集的单位，也是工序挂靠的对象：一条镀膜线、一个抛光班组各是一个工作中心。中心下面挂资源
 * （{@code WorkCenterResource}）——几台机器、几个人、几副模具；日产能 = 这里的每天可用工时 × 资源数量 × 资源效率。
 *
 * <p>价格不在这里——同一天镀膜线给 A 物料计 80 元/小时、给 B 物料计 120 元/小时是正常需求，
 * 费率属于「这道工序在这条路线里怎么用」，见 {@code RoutingOperation}。
 */
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

  /** 每天可用工时（分钟）：中心下的每个资源每天干这么长时间，例如一天开 8 小时就是 480 */
  @Column(name = "CapacityMinutesPerDay", nullable = false, precision = 18, scale = 2)
  private BigDecimal capacityMinutesPerDay;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 资源明细 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "WorkCenterId")
  @SQLRestriction("IsDeleted = 0")
  private List<WorkCenterResource> workCenterResources = new ArrayList<>();

  /**
   * 获取资源明细（只读）
   *
   * @return 不可变列表
   */
  public List<WorkCenterResource> getWorkCenterResources() {
    return Collections.unmodifiableList(workCenterResources);
  }

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  WorkCenter(
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, capacityMinutesPerDay, disabled, remark);
  }

  /**
   * 修改工作中心
   *
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, capacityMinutesPerDay, disabled, remark);
  }

  /**
   * 添加资源
   *
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 资源代码在当前工作中心内已存在时抛出
   */
  public void addResource(
      String code,
      String name,
      long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    WorkCenterResource resource =
        new WorkCenterResource(
            code, name, workCenterResourceCategoryId, quantity, efficiency, disabled, remark);
    if (findResourceByCode(resource.getCode()) != null) {
      throw new DomainException("工作中心资源代码" + resource.getCode() + "已存在!");
    }
    workCenterResources.add(resource);
  }

  /**
   * 修改资源
   *
   * @param id 资源Id
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 资源不存在、或代码在当前工作中心内已存在时抛出
   */
  public void modifyResource(
      long id,
      String code,
      String name,
      long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    WorkCenterResource target = findResource(id);
    if (target == null) {
      throw new DomainException("工作中心资源" + id + "不存在!");
    }
    String trimmedCode = StringUtils.trimToNull(code);
    if (trimmedCode != null && findResourceByCode(trimmedCode, id) != null) {
      throw new DomainException("工作中心资源代码" + trimmedCode + "已存在!");
    }
    target.modify(code, name, workCenterResourceCategoryId, quantity, efficiency, disabled, remark);
  }

  /**
   * 删除资源（逻辑删除）
   *
   * @param id 资源Id
   */
  public void deleteResource(long id) {
    WorkCenterResource target = findResource(id);
    if (target == null) {
      return;
    }
    target.delete();
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 按Id查找未删除的资源
   *
   * @param id 资源Id
   * @return 匹配到的资源，找不到返回 {@code null}
   */
  @Nullable
  private WorkCenterResource findResource(long id) {
    return workCenterResources.stream()
        .filter(i -> Objects.equals(i.getId(), id) && !i.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 按代码查找未删除的资源
   *
   * @param code 资源代码
   * @return 匹配到的资源，找不到返回 {@code null}
   */
  @Nullable
  private WorkCenterResource findResourceByCode(String code) {
    return workCenterResources.stream()
        .filter(i -> !i.getIsDeleted() && code.equals(i.getCode()))
        .findFirst()
        .orElse(null);
  }

  /**
   * 按代码查找未删除的资源，排除指定Id
   *
   * @param code 资源代码
   * @param excludeId 排除的资源Id
   * @return 匹配到的资源，找不到返回 {@code null}
   */
  @Nullable
  private WorkCenterResource findResourceByCode(String code, long excludeId) {
    return workCenterResources.stream()
        .filter(
            i ->
                !i.getIsDeleted()
                    && code.equals(i.getCode())
                    && !Objects.equals(i.getId(), excludeId))
        .findFirst()
        .orElse(null);
  }

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 数值非负校验 → 赋值」收敛到一处，避免构造器和
   * {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  private void apply(
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("工作中心代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工作中心名称不能为空");
    }
    if (capacityMinutesPerDay.signum() <= 0) {
      throw new DomainException("每天可用工时必须大于0");
    }
    // 列是 scale=2，但产能按整数分钟取用，带小数会当场崩：7 小时半要写 450，不许写 450.5
    if (capacityMinutesPerDay.remainder(BigDecimal.ONE).signum() != 0) {
      throw new DomainException("每天可用工时必须是整数分钟");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.capacityMinutesPerDay = capacityMinutesPerDay;
    this.disabled = disabled;
    this.remark = remark;
  }
}
