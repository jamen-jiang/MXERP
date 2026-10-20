package com.mxerp.basedata.domain.variant;

import com.mxerp.basedata.domain.productattribute.AssignedProductAttributeData;
import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.util.HashUtils;
import com.mxerp.shared.domain.util.ValueTextUtils;
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
import java.util.Map;
import java.util.TreeMap;
import java.util.stream.Collectors;
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
 * Variant（一条规格型号，业务口中就是 SKU）
 *
 * <p>这里只放「这个规格是什么」：OPC、规格属性值、单位。采购价、币种、起订量不在这里，见
 * {@code com.mxerp.basedata.domain.supplierprice.SupplierPrice}——同一个规格可以被多个供应商报价，
 * 钉在规格上只能留下最新一个值。
 *
 * <p>值分两处存，是因为它们的作用根本不同：{@link VariantAttributeValue} 是这一行的身份
 * （哪一组值构成一个规格），一行一个取值，改它就等于换一条规格，所以 {@code modify} 不碰它，
 * 唯一性由这些行算出 {@code AttributeSignature} 来保证；描述性取值另存通用赋值表
 * {@code AttributeAssignment}（{@code EntityType = 'Variant'}，定义在 {@code Attribute}，相当于 SAP 的
 * {@code CABN} 与 {@code AUSP}），改它不影响身份。
 * Odoo 也是这个分法：变体组合由 {@code product.template.attribute.line} 展开成 {@code product.product}
 * 的一行，其余描述字段不进入组合唯一性（{@code product_product_combination_unique}）。
 *
 * <p>那张赋值表已挪到平台层 {@code com.mxerp.system.domain.attribute}，本模块不依赖 system-domain，
 * 所以规格聚合上的属性挂载暂时停用，见字段处注释。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Variant",
    indexes = {
      @Index(name = "IX_Variant_ProductId", columnList = "ProductId"),
      @Index(name = "IX_Variant_Opc", columnList = "Opc")
      // 注意：AttributeSignature 的过滤唯一索引带 WHERE IsDeleted=0，用 Flyway 创建
      // 注意：ProductId + Opc 的过滤唯一索引带 WHERE IsDeleted=0 AND Opc IS NOT NULL AND Opc<>''，用 Flyway 创建
    })
public class Variant extends UpdateAggregateRoot {

  /** 所属产品Id */
  @Column(name = "ProductId", nullable = false)
  private Long productId;

  /** 规格属性标识码 */
  @Nationalized
  @Column(name = "AttributeSignature", nullable = false, length = 100)
  private String attributeSignature;

  /** OPC（可空） */
  @Nationalized
  @Column(name = "Opc", length = 100)
  @Nullable
  private String opc;

  /** 单位Id（可空） */
  @Column(name = "UnitId")
  @Nullable
  private Long unitId;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

  /**
   * 规格属性值行（构成规格身份，参与唯一性判断，只读）
   *
   * <p>一个取值一行，所以同一个代码可以有多行；这一组行合起来才是「这个规格是什么」。
   */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "VariantId")
  @SQLRestriction("IsDeleted = 0")
  private List<VariantAttributeValue> productAttributes = new ArrayList<>();

  // 属性值挂载先停用：属性那套（Attribute / AttributeAssignment）已挪到平台层
  // com.mxerp.system.domain.attribute，basedata-domain 不依赖 system-domain，所以这里整段注释保留；
  // 要接回来时先定下「谁来写这张赋值表」，再恢复本段与 VariantDomainService 里的 resolveAttributes。
  // /**
  //  * 属性值（一行一个值，不参与唯一性判断，只读）
  //  *
  //  * <p>与产品的属性值共用一张通用表 {@code AttributeAssignment}，靠 {@code EntityType = 'Variant'} +
  //  * {@code EntityId} 认归属；这一条规格由哪组值构成是 {@code productAttributes} 的事，这里只是补充描述。
  //  */
  // @OneToMany(
  //     fetch = FetchType.EAGER,
  //     cascade = {CascadeType.PERSIST, CascadeType.MERGE},
  //     orphanRemoval = true)
  // @JoinColumn(name = "EntityId")
  // @SQLRestriction("IsDeleted = 0 AND EntityType = 'Variant'")
  // private List<AttributeAssignment> attributes = new ArrayList<>();

  /**
   * 获取规格属性值行（只读）
   *
   * @return 不可变列表
   */
  public List<VariantAttributeValue> getProductAttributes() {
    return Collections.unmodifiableList(productAttributes);
  }

  /**
   * 获取规格属性值（代码 → 值）
   *
   * <p>一个代码只有一行时给出标量，多行时给出列表，读的人不必先数行数。
   *
   * @return 按代码排序的规格属性值
   */
  public TreeMap<String, Object> productAttributeValues() {
    TreeMap<String, Object> result = new TreeMap<>();
    for (Map.Entry<String, List<String>> entry : groupValues().entrySet()) {
      List<String> values = entry.getValue();
      result.put(entry.getKey(), values.size() == 1 ? values.getFirst() : List.copyOf(values));
    }
    return result;
  }

  // /**
  //  * 获取属性值行（只读）
  //  *
  //  * @return 不可变列表
  //  */
  // public List<AttributeAssignment> getAttributes() {
  //   return Collections.unmodifiableList(attributes);
  // }

  // /**
  //  * 同步属性值（按「代码 + 值」匹配：删除不在输入中的、添加新的，已存在的行不动）
  //  *
  //  * <p>规格属性值不参与同步——它们构成这一行的身份，改一组值等于换一条规格，只能走删除 + 重新生成。
  //  *
  //  * @param attributes 目标属性值行数据（可空，为 null 表示这一笔不改动属性值）
  //  */
  // public void syncAttributes(@Nullable List<AssignedAttributeData> attributes) {
  //   if (attributes == null) {
  //     return;
  //   }
  //   List<AssignedAttributeData> target = attributes;

  //   // 1. 删除输入里没有的行
  //   for (AttributeAssignment attribute : this.attributes) {
  //     boolean keep =
  //         target.stream()
  //             .anyMatch(
  //                 d ->
  //                     d.code().equals(attribute.getCode())
  //                         && d.value().equals(attribute.getValue()));
  //     if (!keep) {
  //       attribute.delete();
  //     }
  //   }

  //   // 2. 添加输入里有、当前没有的行
  //   for (AssignedAttributeData data : target) {
  //     boolean exists =
  //         this.attributes.stream()
  //             .anyMatch(i -> !i.getIsDeleted() && i.isSame(data.code(), data.value()));
  //     if (!exists) {
  //       this.attributes.add(
  //           new AttributeAssignment(
  //               "Variant",
  //               data.attributeId(),
  //               data.code(),
  //               data.value()));
  //     }
  //   }
  // }

  /**
   * 构造器
   *
   * @param productId ProductId
   * @param opc OPC（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param productAttributeRows 规格属性值行数据——这一条规格由哪组值构成（可空）
   */
  Variant(
      Long productId,
      @Nullable String opc,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable List<AssignedProductAttributeData> productAttributeRows) {
    // 不可变字段：ProductId 与规格属性值行（决定唯一性签名）创建后不允许修改
    this.productId = productId;
    if (productAttributeRows != null) {
      for (AssignedProductAttributeData data : productAttributeRows) {
        this.productAttributes.add(
            new VariantAttributeValue(data.productAttributeId(), data.code(), data.value()));
      }
    }

    // 可变字段交给 apply 统一处理
    apply(opc, unitId, taxRateId);

    // 签名依赖 productId + 规格属性值行，必须在两者赋值完成后计算
    this.attributeSignature = calculateSignature();
  }

  /**
   * 修改
   *
   * @param opc OPC（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   */
  void modify(@Nullable String opc, @Nullable Long unitId, @Nullable Long taxRateId) {
    apply(opc, unitId, taxRateId);
  }

  /** 启用 */
  public void enable() {
    this.disabled = false;
  }

  /** 禁用 */
  public void disable() {
    this.disabled = true;
  }

  /** 删除 */
  @Override
  public void delete() {
    super.delete();
    addDomainEvent(new VariantDeletedEvent(this));
  }

  // ---------- 签名计算 ----------

  /**
   * 静态工具方法：在不创建实例的情况下算签名，用于按组合查已有 SKU
   *
   * <p>参与拼接的是「规格属性代码=规格属性值」而不是只有值，否则 {@code {SIZE:25}} 与 {@code {COLOR:25}}
   * 会算出同一个签名。
   *
   * @param productId ProductId
   * @param productAttributeRows 规格属性值行数据（可空）
   * @return MD5 特征值
   */
  public static String calculateSignature(
      Long productId, @Nullable List<AssignedProductAttributeData> productAttributeRows) {
    Map<String, List<String>> grouped = new TreeMap<>();
    if (productAttributeRows != null) {
      for (AssignedProductAttributeData data : productAttributeRows) {
        grouped.computeIfAbsent(data.code(), code -> new ArrayList<>()).add(data.value());
      }
    }
    return joinSignature(productId, grouped);
  }

  /**
   * 私有方法：按当前规格属性值行计算签名
   *
   * @return MD5 特征值
   */
  private String calculateSignature() {
    return joinSignature(productId, groupValues());
  }

  /**
   * 当前有效规格属性值行按代码归并
   *
   * @return 代码 → 值列表，按代码排序
   */
  private Map<String, List<String>> groupValues() {
    Map<String, List<String>> grouped = new TreeMap<>();
    for (VariantAttributeValue row : productAttributes) {
      if (Boolean.TRUE.equals(row.getIsDeleted())) {
        continue;
      }
      grouped.computeIfAbsent(row.getCode(), code -> new ArrayList<>()).add(row.getValue());
    }
    return grouped;
  }

  /**
   * 把「代码 → 值列表」拼成签名原文再取 MD5
   *
   * <p>同一代码的多个值先规范再排序，否则同一个组合换个录入顺序就成了另一条规格。
   *
   * @param productId ProductId
   * @param grouped 代码 → 值列表（已按代码排序）
   * @return MD5 特征值
   */
  private static String joinSignature(Long productId, Map<String, List<String>> grouped) {
    StringBuilder builder = new StringBuilder(String.valueOf(productId));
    String joined =
        grouped.entrySet().stream()
            .map(entry -> entry.getKey() + "=" + valuesText(entry.getValue()))
            .collect(Collectors.joining("|"));
    if (!joined.isEmpty()) {
      builder.append('|').append(joined);
    }
    return HashUtils.computeMd5(builder.toString());
  }

  /**
   * 把同一代码的多个规格属性值规范成签名文本
   *
   * <p>{@code 25}、{@code 25.00}、{@code "25"} 都规范成 {@code "25"}，手填的和区间展开的才能算出
   * 同一条规格。
   *
   * @param values 同一代码下的规格属性值
   * @return 排序后用逗号拼接的文本
   */
  private static String valuesText(List<String> values) {
    List<String> texts = values.stream().map(ValueTextUtils::canonicalText).sorted().toList();
    return String.join(",", texts);
  }

  // ---------- 变更字段 ----------

  /**
   * 统一的可变字段赋值入口（构造器与修改共用）
   *
   * <p>{@code productId}、规格属性值行属于不可变字段（决定唯一性签名），由调用方单独赋值；其余字段全部在此
   * 收敛，避免构造器与 {@link #modify} 两处各写一遍导致漏改。
   *
   * @param opc OPC（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   */
  private void apply(@Nullable String opc, @Nullable Long unitId, @Nullable Long taxRateId) {
    this.opc = StringUtils.trimToNull(opc);
    this.unitId = unitId;
    this.taxRateId = taxRateId;
  }
}
