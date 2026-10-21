package com.mxerp.basedata.domain.category;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.entity.Parent;
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
 * 物料分类（树）
 *
 * <p>用来替代硬编码的物料类型枚举。对过外部系统：SAP 的物料组 {@code MARA-MATKL}、Odoo 的
 * {@code material.category}、ERPNext 的 Item Group、金蝶的物料分组、用友的存货分类，无一例外是
 * 「用户自己维护的树」而不是写死在代码里的枚举——因为产品线一变，枚举就得改代码、发版本。
 *
 * <p>分类只回答「这个物料归到哪一类」，进而决定它挂哪一组规格属性；「这个物料怎么来」（自制/外购）
 * 是另一个维度，见 {@code ProcurementTypeEnum}。SAP 把 {@code MATKL} 与 {@code MARC-BESKZ}
 * 分成两个字段正是这个原因，两者可以任意组合，不要混在一个字段里。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Category",
    indexes = {
      @Index(name = "IX_Category_ParentId", columnList = "ParentId"),
      @Index(name = "IX_Category_Level", columnList = "Level"),
      @Index(name = "IX_Category_Name", columnList = "Name"),
      @Index(name = "IX_Category_Sort", columnList = "Sort")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Category extends UpdateAggregateRoot implements Parent {

  /** 层级路径（可空，形如 "1,3,5"，由层级编排逻辑维护） */
  @Nationalized
  @Column(name = "Level")
  @Nullable
  private String level;

  /** 父分类Id（可空，根节点为空） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 分类代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 分类名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

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
   * @param level 层级路径（可空）
   * @param parentId 父分类Id（可空）
   * @param code 分类代码
   * @param name 分类名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  Category(
      @Nullable String level,
      @Nullable Long parentId,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(level, parentId, code, name, disabled, sort, remark);
  }

  /**
   * 修改分类
   *
   * @param level 层级路径（可空）
   * @param parentId 父分类Id（可空）
   * @param code 分类代码
   * @param name 分类名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      @Nullable String level,
      @Nullable Long parentId,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(level, parentId, code, name, disabled, sort, remark);
  }

  /**
   * 重设层级路径
   *
   * @param level 层级路径（可空）
   */
  @Override
  public void changeLevel(@Nullable String level) {
    this.level = level;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param level 层级路径（可空）
   * @param parentId 父分类Id（可空）
   * @param code 分类代码
   * @param name 分类名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      @Nullable String level,
      @Nullable Long parentId,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("分类代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("分类名称不能为空");
    }
    this.level = level;
    this.parentId = parentId;
    this.code = trimmedCode;
    this.name = trimmedName;
    this.disabled = disabled;
    this.sort = sort;
    this.remark = remark;
  }
}
