package com.mxerp.basedata.domain.product;

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
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.TreeMap;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 产品
 *
 * <p>这里只放「产品是什么」：分类、代码、名称、单位。采购价、币种、起订量不在这里——那是「供应商 × 产品」
 * 这一层的事，一个产品可以有多个供应商和多个报价，钉在产品上只能留下最新一个值，历史价当场丢失。见
 * {@code com.mxerp.basedata.domain.supplierprice.SupplierPrice}。
 *
 * <p>{@code categoryId} 与 {@code procurementType} 是两个正交维度：前者说「归哪一类、挂哪组规格属性」，
 * 后者说「自制还是外购」。SAP 用 {@code MARA-MATKL} 与 {@code MARC-BESKZ} 两个字段分开表达，
 * 不能合并成一个「产品类型」枚举——同分类的东西既可能自制也可能外购。
 *
 * <p>产品上填的属性值放通用赋值表 {@code AttributeAssignment}（{@code EntityType = 'Product'}），不存 JSON
 * 列：一行一个值才能按代码查、按值排序，也能让同一代码有多个值（比如颜色）。定义在 {@code Attribute}，
 * 赋值在 {@code AttributeAssignment}，相当于 SAP 把 {@code CABN}（定义）和 {@code AUSP}（赋值，
 * 带 {@code OBJ_TYPE} + {@code OBJ_ID}）分在两张表。这张赋值表已挪到平台层
 * {@code com.mxerp.system.domain.attribute}，产品聚合上的挂载暂时停用，见字段处注释。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Product",
    indexes = {
      @Index(name = "IX_Product_CategoryId", columnList = "CategoryId"),
      @Index(name = "IX_Product_Group", columnList = "[Group]"),
      @Index(name = "IX_Product_SupplierId", columnList = "SupplierId"),
      @Index(name = "IX_Product_Code", columnList = "Code"),
      @Index(name = "IX_Product_Name", columnList = "Name")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，用 Flyway 创建
    })
public class Product extends UpdateAggregateRoot {

  /** 所属分类Id */
  @Column(name = "CategoryId", nullable = false)
  private Long categoryId;

  /** 取得方式：自制、外购、两者皆可 */
  @Nationalized
  @Column(name = "ProcurementType", nullable = false, length = 50)
  private String procurementType;

  /** 供应商Id（可空，外购时的默认供应商；自制可以留空） */
  @Column(name = "SupplierId")
  @Nullable
  private Long supplierId;

  /** 品牌Id（可空） */
  @Column(name = "BrandId")
  @Nullable
  private Long brandId;

  /** 产品代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 100)
  private String code;

  /** 产品名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 200)
  private String name;

  /** 产品描述（可空） */
  @Nationalized
  @Column(name = "Description", length = 200)
  @Nullable
  private String description;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  // 属性值挂载先停用：属性那套（Attribute / AttributeAssignment）已挪到平台层
  // com.mxerp.system.domain.attribute，basedata-domain 不依赖 system-domain，所以这里整段注释保留；
  // 要接回来时先定下「谁来写这张赋值表」，再恢复本段与 ProductDomainService 里的 resolveAttributes。
//  /**
//   * 属性值（一行一个值，只读）
//   *
//   * <p>与规格的属性值共用一张通用表 {@link AttributeAssignment}，靠 {@code EntityType = 'Product'} +
//   * {@code EntityId} 认归属；关联列写的是 {@code EntityId}，{@code EntityType} 由
//   * {@link #syncAttributes} 建行时给出。
//   */
//  @OneToMany(
//      fetch = FetchType.EAGER,
//      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
//      orphanRemoval = true)
//  @JoinColumn(name = "EntityId")
//  @SQLRestriction("IsDeleted = 0 AND EntityType = 'Product'")
//  private List<AttributeAssignment> attributes = new ArrayList<>();
//
//  /**
//   * 获取属性值行（只读）
//   *
//   * @return 不可变列表
//   */
//  public List<AttributeAssignment> getAttributes() {
//    return Collections.unmodifiableList(attributes);
//  }
//
//  /**
//   * 属性值还原成「代码 → 值」的 Map
//   *
//   * <p>一个代码一行时给出标量，多行时给出列表。
//   *
//   * @return 属性值 Map
//   */
//  public TreeMap<String, Object> attributeValues() {
//    return AttributeUtils.toMap(attributes);
//  }
//
//  /**
//   * 同步属性值（按「代码 + 值」匹配：删除不在输入中的、添加新的，已存在的行不动）
//   *
//   * <p>不做「更新」是因为行的身份就是「代码 + 值」本身，改值等于换一行。
//   *
//   * @param attributes 目标属性值行数据（可空，为 null 表示这一笔不改动属性值）
//   */
//  public void syncAttributes(@Nullable List<AssignedAttributeData> attributes) {
//    if (attributes == null) {
//      return;
//    }
//    List<AssignedAttributeData> target = attributes;
//
//    // 1. 删除输入里没有的行
//    for (AttributeAssignment attribute : this.attributes) {
//      boolean keep =
//          target.stream()
//              .anyMatch(
//                  d ->
//                      d.code().equals(attribute.getCode())
//                          && d.value().equals(attribute.getValue()));
//      if (!keep) {
//        attribute.delete();
//      }
//    }
//
//    // 2. 添加输入里有、当前没有的行
//    for (AssignedAttributeData data : target) {
//      boolean exists =
//          this.attributes.stream()
//              .anyMatch(i -> !i.getIsDeleted() && i.isSame(data.code(), data.value()));
//      if (!exists) {
//        this.attributes.add(
//            new AttributeAssignment(
//                "Product",
//                data.attributeId(),
//                data.code(),
//                data.value()));
//      }
//    }
//  }

  /**
   * 构造器
   *
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attachmentIds 附件Id集合（可空）
   */
  Product(
      Long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      Long unitId,
      @Nullable Long taxRateId,
      Boolean disabled,
      @Nullable String remark,
      @Nullable List<Long> attachmentIds) {
    apply(
        categoryId,
        procurementType,
        supplierId,
        brandId,
        code,
        name,
        description,
        unitId,
        taxRateId,
        disabled,
        remark);
    addDomainEvent(new ProductSavedEvent(this, attachmentIds));
  }

  /**
   * 修改
   *
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attachmentIds 附件Id集合（可空）
   */
  void modify(
      Long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      Long unitId,
      @Nullable Long taxRateId,
      Boolean disabled,
      @Nullable String remark,
      @Nullable List<Long> attachmentIds) {
    apply(
        categoryId,
        procurementType,
        supplierId,
        brandId,
        code,
        name,
        description,
        unitId,
        taxRateId,
        disabled,
        remark);
    addDomainEvent(new ProductSavedEvent(this, attachmentIds));
  }

  /** 删除 */
  @Override
  public void delete() {
    super.delete();
    addDomainEvent(new ProductDeletedEvent(this));
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   */
  private void apply(
      Long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      Long unitId,
      @Nullable Long taxRateId,
      Boolean disabled,
      @Nullable String remark) {
    if (categoryId == null) {
      throw new DomainException("产品分类不能为空");
    }
    validateProcurementType(procurementType);

    this.categoryId = categoryId;
    this.procurementType = procurementType;
    this.supplierId = supplierId;
    this.brandId = brandId;
    this.code = code;
    this.name = name;
    this.description = description;
    this.unitId = unitId;
    this.taxRateId = taxRateId;
    this.disabled = disabled;
    this.remark = remark;
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验取得方式
   *
   * @param procurementType 取得方式代码
   * @throws DomainException 不在 {@link ProcurementTypeEnum} 范围内时抛出
   */
  private void validateProcurementType(String procurementType) {
    if (EnumUtils.fromCode(ProcurementTypeEnum.class, procurementType) == null) {
      throw new DomainException("取得方式" + procurementType + "有误!");
    }
  }
}
