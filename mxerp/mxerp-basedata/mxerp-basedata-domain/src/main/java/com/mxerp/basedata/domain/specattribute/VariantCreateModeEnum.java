package com.mxerp.basedata.domain.specattribute;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 规格属性的变体生成方式 */
@NullMarked
@Getter
public enum VariantCreateModeEnum implements BaseEnum {

  /** 立即生成：保存规格属性值时就按组合把所有 SKU 建出来 */
  ALWAYS("Always", "立即生成"),

  /** 按需生成：不预建 SKU，单据选到这组规格时再建 */
  DYNAMIC("Dynamic", "按需生成");

  /** 编码 */
  private final String code;

  /** 标签 */
  private final String label;

  /**
   * 构造器
   *
   * @param code 编码
   * @param label 标签
   */
  VariantCreateModeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
