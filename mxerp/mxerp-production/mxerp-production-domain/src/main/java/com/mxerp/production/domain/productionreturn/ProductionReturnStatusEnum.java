package com.mxerp.production.domain.productionreturn;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 生产退料单状态 */
@NullMarked
@Getter
public enum ProductionReturnStatusEnum implements BaseEnum {

  /** 草稿：还在申请，既没动库存也没动工单用料 */
  DRAFT("Draft", "草稿"),

  /** 已退料：核定数量真回到仓库了，单据落定 */
  POSTED("Posted", "已退料");

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
  ProductionReturnStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
