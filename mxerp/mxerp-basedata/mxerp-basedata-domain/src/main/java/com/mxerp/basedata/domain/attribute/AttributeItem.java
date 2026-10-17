package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
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

/** 属性项 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "AttributeItem",
    indexes = {
      @Index(name = "IX_AttributeItem_AttributeId", columnList = "AttributeId"),
      @Index(name = "IX_AttributeItem_Value", columnList = "Value"),
      @Index(name = "IX_AttributeItem_Sort", columnList = "Sort")
      // 注意：AttributeId + Value 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class AttributeItem extends BaseDeleteEntity {

  /** 属性Id */
  @Column(name = "AttributeId")
  private Long attributeId;

  /** 属性值（如：-25,-50） */
  @Nationalized
  @Column(name = "Value", nullable = false, length = 50)
  private String value;

  /** 属性值名称（如：-25,-50） */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否默认 */
  @Column(name = "IsDefault", nullable = false)
  private Boolean isDefault;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /**
   * 构造器
   *
   * @param value 属性值
   * @param name 属性值名称
   * @param isDefault 是否默认
   * @param sort 排序（可空）
   */
  AttributeItem(String value, String name, Boolean isDefault, @Nullable Integer sort) {
    String trimmedValue = StringUtils.trimToNull(value);
    if (trimmedValue == null) {
      throw new DomainException("属性值不能为空");
    }
    // value 是属性项的业务主键，仅在创建时确定，之后不可变
    this.value = trimmedValue;
    apply(name, isDefault, sort);
  }

  /**
   * 修改属性项
   *
   * @param name 属性值名称
   * @param isDefault 是否默认
   * @param sort 排序（可空）
   */
  void modify(String name, Boolean isDefault, @Nullable Integer sort) {
    apply(name, isDefault, sort);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的可变字段校验与赋值入口（构造器与修改共用）
   *
   * <p>{@code value} 创建后不可变，因此这里只处理名称、默认标记与排序三个可变字段，避免两处各写一遍。
   *
   * @param name 属性值名称
   * @param isDefault 是否默认
   * @param sort 排序（可空）
   */
  private void apply(String name, Boolean isDefault, @Nullable Integer sort) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("属性值名称不能为空");
    }
    this.name = trimmedName;
    this.isDefault = isDefault;
    this.sort = sort;
  }

  /** 设置为默认项 */
  void setAsDefault() {
    this.isDefault = true;
  }

  /** 取消默认项 */
  void unsetDefault() {
    this.isDefault = false;
  }
}
