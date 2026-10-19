package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 炉号单状态
 *
 * <p>这是一张成本归集单，不是一张车间执行单，所以状态只跟着「账定没定」走，不跟物理动作走：草稿期结构与口径
 * 都能改，过账把这一炉的工时与费用摊到各行，红冲是过账的对称撤销。作废只发生在过账之前。
 *
 * <p>装炉、升温、出炉这些动作留在工单工序的报工里，本单据不重复记一遍——一炉做三张单时，物理进度已经由那三张
 * 单各自报过了，这里再设一档就是两处维护同一件事，早晚对不上。
 */
@NullMarked
@Getter
public enum ProductionRunStatusEnum implements BaseEnum {

  /** 草稿：行与分配标准都还能改，尚未影响任何账 */
  DRAFT("Draft", "草稿"),

  /** 已过账：本炉工时与费用已按各行占用量摊定，此后只能红冲 */
  POSTED("Posted", "已过账"),

  /** 已取消：过账之前作废，占用的工单工序一并释放 */
  CANCELLED("Cancelled", "已取消"),

  /** 已红冲：{@code POSTED} 的对称撤销，摊后数清零，占用一并释放 */
  REVERSED("Reversed", "已红冲");

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
  ProductionRunStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
