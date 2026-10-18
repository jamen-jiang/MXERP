package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** BOM 行的发料方式 */
@NullMarked
@Getter
public enum IssueModeEnum implements BaseEnum {

  /** 按件：每片成品消耗固定量（毛坯、包装袋） */
  PIECE("Piece", "按件"),

  /** 按批：按生产批次整批领用（染色剂） */
  BATCH("Batch", "按批");

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
  IssueModeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
