package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 发料触发：这条用料由谁发起领料、在哪个动作上自动扣
 *
 * <p>一列表达完「要不要倒冲」和「什么时候倒冲」，跟金蝶的「发料方式」（直接领料 / 直接倒冲 / 不发料）和
 * 用友的「供应类型」（工序倒冲 / 入库倒冲）一样，不另开一个是否倒冲的标志列。手工领料是默认值，等价于不倒冲。
 *
 * <p>这根轴与 {@link IssueModeEnum} 无关：发料方式说「用量怎么乘」（按件还是整批），触发说「谁来发起」。
 */
@NullMarked
@Getter
public enum IssueTriggerEnum implements BaseEnum {

  /** 手工领料：车间自己开领料单，系统不自动生成 */
  MANUAL("Manual", "手工领料"),

  /** 报工倒冲：每报一次工，按本次产出量自动生成一张领料单 */
  REPORT("Report", "报工倒冲"),

  /** 入库倒冲：完工入库时按本次实收量自动生成一张领料单 */
  RECEIPT("Receipt", "入库倒冲");

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
  IssueTriggerEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
