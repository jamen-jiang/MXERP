package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 属性范围 */
@NullMarked
@Getter
public enum AttributeScopeEnum implements BaseEnum {

  /** 产品 */
  SPU("Spu", "产品"),

  /** Sku */
  SKU("Sku", "Sku"),

  /** 销售产品 */
  SALES_SPU("SalesSpu", "销售产品"),

  /** 销售Sku */
  SALES_SKU("SalesSku", "销售Sku"),

  /** 公用 */
  COMMON("Common", "公用");

  /** 编码 */
  private final String code;

  /** 名称 */
  private final String name;

  /**
   * 构造器
   *
   * @param code 编码
   * @param name 名称
   */
  AttributeScopeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
