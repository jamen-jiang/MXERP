package com.mxerp.basedata.domain.suppliergroup;

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
 * 供应商分类
 *
 * <p>金蝶云星空的「供应商分类」：分类编码、名称、上级分类（树形），供应商少可以不维护分类。
 * 供应商挂在分类下主要是为了归类查询和按分类配默认值；与 SAP 的科目组 {@code KTOKK}、
 * D365 的 Vendor group 不是一回事——那两个还决定科目和编号段，分类只管归类。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "SupplierGroup",
    indexes = {
      @Index(name = "IX_SupplierGroup_ParentId", columnList = "ParentId"),
      @Index(name = "IX_SupplierGroup_ParentPath", columnList = "ParentPath"),
      @Index(name = "IX_SupplierGroup_Code", columnList = "Code"),
      @Index(name = "IX_SupplierGroup_Name", columnList = "Name"),
      @Index(name = "IX_SupplierGroup_Sequence", columnList = "Sequence")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class SupplierGroup extends UpdateAggregateRoot implements Parent {

  /** 上级分类Id（可空，空表示顶级） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 层级路径（可空，形如 "1-3-5"，由层级编排逻辑维护） */
  @Nationalized
  @Column(name = "ParentPath")
  @Nullable
  private String parentPath;

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 排序（可空） */
  @Column(name = "Sequence")
  @Nullable
  private Integer sequence;

  /** 是否禁用 */
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
   * @param parentPath 层级路径（可空）
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  SupplierGroup(
      @Nullable String parentPath,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    apply(parentPath, code, name, parentId, disabled, sequence, remark);
  }

  /**
   * 修改供应商分类
   *
   * @param parentPath 层级路径（可空）
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      @Nullable String parentPath,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    apply(parentPath, code, name, parentId, disabled, sequence, remark);
  }

  /**
   * 重设层级路径
   *
   * @param parentPath 层级路径（可空）
   */
  @Override
  public void changeParentPath(@Nullable String parentPath) {
    this.parentPath = parentPath;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param parentPath 层级路径（可空）
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空时抛出
   */
  private void apply(
      @Nullable String parentPath,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("供应商分类代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("供应商分类名称不能为空");
    }

    this.parentPath = parentPath;
    this.code = trimmedCode;
    this.name = trimmedName;
    this.parentId = parentId;
    this.disabled = disabled;
    this.sequence = sequence;
    this.remark = remark;
  }
}
