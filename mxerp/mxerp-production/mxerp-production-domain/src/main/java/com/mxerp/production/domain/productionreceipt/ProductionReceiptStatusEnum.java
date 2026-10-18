package com.mxerp.production.domain.productionreceipt;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 生产入库单状态 */
@NullMarked
@Getter
public enum ProductionReceiptStatusEnum implements BaseEnum {

  /** 草稿：车间刚把货报上来，库存和工单都还没动 */
  DRAFT("Draft", "草稿"),

  /** 已入库：实收数量真进了成品仓，单据落定 */
  POSTED("Posted", "已入库");

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
  ProductionReceiptStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
