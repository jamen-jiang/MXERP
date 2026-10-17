package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.enums.ProductTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.*;
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

/** 属性 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Attribute",
    indexes = {
      @Index(name = "IX_Attribute_ProductType", columnList = "ProductType"),
      @Index(name = "IX_Attribute_Scope", columnList = "Scope"),
      @Index(name = "IX_Attribute_Code", columnList = "Code"),
      @Index(name = "IX_Attribute_Name", columnList = "Name"),
      @Index(name = "IX_Attribute_Sort", columnList = "Sort")
      // 注意：ProductType + Scope + Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Attribute extends UpdateAggregateRoot {

  /** 产品类型 */
  @Nationalized
  @Column(name = "ProductType", nullable = false, length = 50)
  private String productType;

  /** 范围:Sku,Spu */
  @Nationalized
  @Column(name = "Scope", nullable = false, length = 50)
  private String scope;

  /** 属性代码（如：SPH,CYL） */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 属性名称（如：SPH,CYL） */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否关键属性（参与SKU唯一判定） */
  @Column(name = "IsKey", nullable = false)
  private Boolean isKey;

  /** 输入类型:数字输入框、文本框、下拉框、复选框、开关 */
  @Nationalized
  @Column(name = "InputType", nullable = false, length = 50)
  private String inputType;

  /** 数据类型：数值型、文本型、枚举型、布尔型 */
  @Nationalized
  @Column(name = "DataType", nullable = false, length = 50)
  private String dataType;

  /** 是否必填 */
  @Column(name = "IsRequired", nullable = false)
  private Boolean isRequired;

  /** 是否用于筛选 */
  @Column(name = "IsFilter", nullable = false)
  private Boolean isFilter;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 属性项列表（只读） */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "AttributeId")
  @SQLRestriction("IsDeleted = 0")
  private List<AttributeItem> attributeItems = new ArrayList<>();

  /** 校验规则列表（只读） */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "AttributeId")
  @SQLRestriction("IsDeleted = 0")
  private List<AttributeValidateRule> attributeValidateRules = new ArrayList<>();

  /**
   * 获取属性项列表（只读）
   *
   * @return 不可变列表
   */
  public List<AttributeItem> getAttributeItems() {
    return Collections.unmodifiableList(attributeItems);
  }

  /**
   * 获取校验规则列表（只读）
   *
   * @return 不可变列表
   */
  public List<AttributeValidateRule> getAttributeValidateRules() {
    return Collections.unmodifiableList(attributeValidateRules);
  }

  Attribute(
      String productType,
      String scope,
      String code,
      String name,
      Boolean isKey,
      String inputType,
      String dataType,
      Boolean isRequired,
      Boolean isFilter,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedProductType = StringUtils.trimToNull(productType);
    String trimmedScope = StringUtils.trimToNull(scope);
    String trimmedCode = StringUtils.trimToNull(code);
    if (trimmedProductType == null) {
      throw new DomainException("产品类型不能为空");
    }
    if (trimmedScope == null) {
      throw new DomainException("范围不能为空");
    }
    if (trimmedCode == null) {
      throw new DomainException("属性代码不能为空");
    }
    validateProductType(trimmedProductType);
    validateScope(trimmedScope);
    validateDataType(dataType);

    // 不可变字段：产品类型、范围、代码、是否关键属性、数据类型创建后不允许修改，只在构造时赋值
    this.productType = trimmedProductType;
    this.scope = trimmedScope;
    this.code = trimmedCode;
    this.isKey = isKey;
    this.dataType = dataType;

    // 可变字段交给 apply 统一处理
    apply(name, inputType, isRequired, isFilter, sort, remark);
  }

  /**
   * 修改属性
   *
   * <p>只能修改可变字段；产品类型、范围、代码、是否关键属性、数据类型创建后不可变更。
   *
   * @param name 属性名称
   * @param inputType 输入类型
   * @param isRequired 是否必填
   * @param isFilter 是否用于筛选
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      String name,
      String inputType,
      Boolean isRequired,
      Boolean isFilter,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(name, inputType, isRequired, isFilter, sort, remark);
  }

  /**
   * 新增属性项
   *
   * @param value 属性值
   * @param name 属性值名称
   * @param isDefault 是否默认项
   * @param sort 排序（可空）
   * @throws DomainException 属性值在该属性下已存在时抛出
   */
  public void addItem(String value, String name, Boolean isDefault, @Nullable Integer sort) {
    if (isExistItem(value)) {
      throw new DomainException("属性" + this.name + ":属性项" + value + " 已存在!");
    }
    AttributeItem item = new AttributeItem(value, name, isDefault, sort);
    attributeItems.add(item);
  }

  /**
   * 修改属性项
   *
   * <p>属性值（{@code value}）创建后不可修改，这里只能改名称、默认标记与排序。
   *
   * @param itemId 属性项Id
   * @param name 属性值名称
   * @param isDefault 是否默认项
   * @param sort 排序（可空）
   * @throws DomainException 属性项不存在时抛出
   */
  public void modifyItem(Long itemId, String name, Boolean isDefault, @Nullable Integer sort) {
    AttributeItem item =
        attributeItems.stream()
            .filter(i -> Objects.equals(i.getId(), itemId))
            .findFirst()
            .orElse(null);
    if (item == null) {
      throw new DomainException("属性" + this.name + ":属性项不存在!");
    }
    item.modify(name, isDefault, sort);
  }

  /**
   * 删除属性项（逻辑删除）
   *
   * <p>找不到对应属性项时静默返回，保证删除操作幂等。
   *
   * @param itemId 属性项Id
   */
  public void deleteItem(Long itemId) {
    AttributeItem item =
        attributeItems.stream()
            .filter(i -> Objects.equals(i.getId(), itemId))
            .findFirst()
            .orElse(null);
    if (item == null) {
      return;
    }
    item.delete();
  }

  /**
   * 新增校验规则
   *
   * @param expression 校验表达式
   * @param errorMessage 校验失败提示
   * @param sort 排序（可空）
   * @throws DomainException 相同表达式已存在时抛出
   */
  public void addValidateRule(String expression, String errorMessage, @Nullable Integer sort) {
    if (isExistValidateRule(expression)) {
      throw new DomainException("属性" + this.name + ":校验规则已存在!");
    }
    attributeValidateRules.add(new AttributeValidateRule(expression, errorMessage, sort));
  }

  /**
   * 修改校验规则
   *
   * @param validateRuleId 校验规则Id
   * @param expression 校验表达式
   * @param errorMessage 校验失败提示
   * @param sort 排序（可空）
   * @throws DomainException 校验规则不存在时抛出
   */
  public void modifyValidateRule(
      Long validateRuleId, String expression, String errorMessage, @Nullable Integer sort) {
    AttributeValidateRule validateRule =
        attributeValidateRules.stream()
            .filter(r -> Objects.equals(r.getId(), validateRuleId))
            .findFirst()
            .orElse(null);
    if (validateRule == null) {
      throw new DomainException("属性" + this.name + ":校验规则不存在!");
    }
    validateRule.modify(expression, errorMessage, sort);
  }

  /**
   * 删除校验规则（逻辑删除）
   *
   * <p>找不到对应规则时静默返回，保证删除操作幂等。
   *
   * @param validateRuleId 校验规则Id
   */
  public void deleteValidateRule(Long validateRuleId) {
    AttributeValidateRule validateRule =
        attributeValidateRules.stream()
            .filter(r -> Objects.equals(r.getId(), validateRuleId))
            .findFirst()
            .orElse(null);
    if (validateRule == null) {
      return;
    }
    validateRule.delete();
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的可变字段校验与赋值入口（构造器与修改共用）
   *
   * <p>只处理允许修改的字段；产品类型、范围、代码、是否关键属性、数据类型属于不可变字段，由调用方单独赋值。
   *
   * @param name 属性名称
   * @param inputType 输入类型
   * @param isRequired 是否必填
   * @param isFilter 是否用于筛选
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      String name,
      String inputType,
      Boolean isRequired,
      Boolean isFilter,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("属性名称不能为空");
    }
    validateInputType(inputType);

    this.name = trimmedName;
    this.inputType = inputType;
    this.isRequired = isRequired;
    this.isFilter = isFilter;
    this.sort = sort;
    this.remark = remark;
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验产品类型是否合法
   *
   * @param productType 产品类型代码
   * @throws DomainException 类型不在 {@link ProductTypeEnum} 范围内时抛出
   */
  private void validateProductType(String productType) {
    if (EnumUtils.fromCode(ProductTypeEnum.class, productType) == null) {
      throw new DomainException("产品类型" + productType + "有误!");
    }
  }

  /**
   * 校验属性适用范围是否合法
   *
   * @param scope 范围代码（Sku / Spu）
   * @throws DomainException 范围不在 {@link AttributeScopeEnum} 范围内时抛出
   */
  private void validateScope(String scope) {
    if (EnumUtils.fromCode(AttributeScopeEnum.class, scope) == null) {
      throw new DomainException("范围" + scope + "有误!");
    }
  }

  /**
   * 校验输入类型是否合法
   *
   * @param inputType 输入类型代码（数字输入框、文本框、下拉框等）
   * @throws DomainException 类型不在 {@link InputTypeEnum} 范围内时抛出
   */
  private void validateInputType(String inputType) {
    if (EnumUtils.fromCode(InputTypeEnum.class, inputType) == null) {
      throw new DomainException("输入类型" + inputType + "有误!");
    }
  }

  /**
   * 校验数据类型是否合法
   *
   * @param dataType 数据类型代码（数值型、文本型、枚举型、布尔型）
   * @throws DomainException 类型不在 {@link DataTypeEnum} 范围内时抛出
   */
  private void validateDataType(String dataType) {
    if (EnumUtils.fromCode(DataTypeEnum.class, dataType) == null) {
      throw new DomainException("数据类型" + dataType + "有误!");
    }
  }

  // ==================== 私有：存在性判断 ====================

  /**
   * 判断属性值是否已存在于当前属性的属性项列表中
   *
   * <p>已逻辑删除的属性项不参与判断，这样删除后可以重新添加同名属性值。
   *
   * @param value 属性值
   * @return true：已存在
   */
  private boolean isExistItem(String value) {
    return attributeItems.stream().anyMatch(i -> i.getValue().equals(value) && !i.getIsDeleted());
  }

  /**
   * 判断校验规则表达式是否已存在于当前属性的规则列表中
   *
   * <p>已逻辑删除的规则不参与判断，这样删除后可以重新添加相同表达式。
   *
   * @param expression 校验表达式
   * @return true：已存在
   */
  private boolean isExistValidateRule(String expression) {
    return attributeValidateRules.stream()
        .anyMatch(r -> r.getExpression().equals(expression) && !r.getIsDeleted());
  }
}
