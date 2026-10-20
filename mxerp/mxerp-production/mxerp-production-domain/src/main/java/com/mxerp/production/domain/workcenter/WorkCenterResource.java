package com.mxerp.production.domain.workcenter;

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
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工作中心资源（工作中心的子行）
 *
 * <p>工作中心下面实际承载产能的东西：一台机器、一个人、一副模具。工作中心回答「这个单元一天开多久」，
 * 资源回答「这个单元有几台、能干到几成」。日产能 = 工作中心的每天可用工时 × 资源数量 × 资源效率。
 *
 * <p>价格不在这里，见 {@code RoutingOperation} 引用的费率卡。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkCenterResource",
    indexes = {
      @Index(name = "IX_WorkCenterResource_WorkCenterId", columnList = "WorkCenterId"),
      @Index(name = "IX_WorkCenterResource_Code", columnList = "Code"),
      @Index(name = "IX_WorkCenterResource_Name", columnList = "Name"),
      @Index(name = "IX_WorkCenterResource_Category", columnList = "Category"),
      @Index(name = "IX_WorkCenterResource_Disabled", columnList = "Disabled")
      // 注意：WorkCenterId + Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class WorkCenterResource extends BaseDeleteEntity {

  /** 工作中心Id */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 工作中心资源类别Id */
  @Column(name = "WorkCenterResourceCategoryId", nullable = false)
  private Long workCenterResourceCategoryId;

  /** 数量：同时可用的台数或人数（整数） */
  @Column(name = "Quantity", nullable = false)
  private Integer quantity;

  /** 效率（百分比）：100 表示按标准工时满负荷产出 */
  @Column(name = "Efficiency", nullable = false, precision = 5, scale = 2)
  private BigDecimal efficiency;

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
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  WorkCenterResource(
      String code,
      String name,
      Long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, workCenterResourceCategoryId, quantity, efficiency, disabled, remark);
  }

  /**
   * 修改工作中心资源
   *
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      Long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, workCenterResourceCategoryId, quantity, efficiency, disabled, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 数值校验 → 赋值」收敛到一处，避免构造器和
   * {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  private void apply(
      String code,
      String name,
      Long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("工作中心资源代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工作中心资源名称不能为空");
    }
    if (workCenterResourceCategoryId <= 0) {
      throw new DomainException("工作中心资源类别不能为空");
    }
    if (quantity <= 0) {
      throw new DomainException("资源数量必须大于0");
    }
    if (efficiency.signum() <= 0 || efficiency.compareTo(BigDecimal.valueOf(100)) > 0) {
      throw new DomainException("资源效率必须大于0且不超过100");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.workCenterResourceCategoryId = workCenterResourceCategoryId;
    this.quantity = quantity;
    this.efficiency = efficiency;
    this.disabled = disabled;
    this.remark = remark;
  }
}
