package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 计量单位组（同一个物理量纲下的一组单位）
 *
 * <p>换算只允许在组内做：组里挑一个「基准单位」，其余单位用 {@code Unit.factor} 表示
 * 「1 个基准单位 = factor 个本单位」。这样「米 ↔ 厘米」能换，「米 ↔ 公斤」根本没有入口——
 * Odoo 的 {@code uom.category} 是同一套规矩（"only convert when both units belong to the same
 * category"，且一个类别只能有一个 reference 单位），金蝶的「计量单位组」、用友的「计量单位组」也是这么分的；
 * SAP 把量纲放在单位表 {@code T006} 的维度字段上，换算则挂在物料上（{@code MARM}）。
 *
 * <p>单位组是主数据：组名按物理量纲起（长度、重量、面积、时间、数量），不要按业务起
 * （「镜片单位」「辅料单位」），否则同一个「个」会在组之间重复，组内换算立刻说不清。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "UnitGroup",
    indexes = {
      @Index(name = "IX_UnitGroup_Name", columnList = "Name"),
      @Index(name = "IX_UnitGroup_Sort", columnList = "Sort")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class UnitGroup extends UpdateAggregateRoot {

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称（按物理量纲起，如 长度 / 重量 / 数量） */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

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
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  UnitGroup(
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(code, name, disabled, sort, remark);
  }

  /**
   * 修改单位组
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(code, name, disabled, sort, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空时抛出
   */
  private void apply(
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("单位组代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("单位组名称不能为空");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.disabled = disabled;
    this.sort = sort;
    this.remark = remark;
  }
}
