package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 工序工时取时公式
 *
 * <p>参照 Dynamics 365 路由的 Formula：同一道工序的 {@code runTime} 既可以是「单件工时」也可以是「整批工时」，
 * 全看这道工序按什么口径投料。没有这个标志，按批生产的工序会被乘以数量，准备与加工成本直接翻几十倍。
 */
@NullMarked
@Getter
public enum TimeFormulaEnum implements BaseEnum {

  /** 按件：加工工时是单件工时，总工时 = 加工工时 × 数量，准备工时整单只做一次 */
  STANDARD("Standard", "按件"),

  /** 按批：准备与加工工时都是「每批」的量，批次数 = ⌈数量 ÷ 标准批量⌉，不足一批按一批算 */
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
  TimeFormulaEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
