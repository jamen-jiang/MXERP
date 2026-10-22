package com.mxerp.basedata.domain.material;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 物料类型枚举
 *
 * <p>照 Odoo 产品表上的 {@code detailed_type}：库存料要进库存账（Odoo 里由仓储模块加的这一档决定），消耗品买了直接领用、
 * 不跟踪库存，服务压根不是实物。"这张料管不管库存"在 Odoo 不是布尔开关，而是类型里的一档，所以这里也不单独放
 * {@code IsStockItem}。
 *
 * <p>「自制还是外购」不在这一列：Odoo 用产品上的路线（采购、制造、分包）表达，我们对应的是「有没有工艺路线」和
 * 「有没有采购价格」，不额外占一列。
 */
@NullMarked
@Getter
public enum MaterialTypeEnum implements BaseEnum {

  /** 库存料：进出库存账，可上 BOM、可领料 */
  STOCK("Stock", "库存料"),

  /** 消耗品：劳保、办公用品这类，买了直接领用，不进库存账 */
  CONSUMABLE("Consumable", "消耗品"),

  /** 服务：运输、检测、加工费这类非实物 */
  SERVICE("Service", "服务");

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
  MaterialTypeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
