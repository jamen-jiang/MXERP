package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 分配标准：一炉的工时与费用按什么口径分给涉及的工单
 *
 * <p>这一档不是我发明的口径，是《企业产品成本核算制度（试行）》（财会〔2013〕17号）第36条说的话：制造费用要按
 * 「机器工时、人工工时、计划分配率」这类分配标准分，第37条给联副产品的则是「系数法、实物量法、相对销售价格法」。
 * 炉号单就是把这一炉的费用按这些标准分一次的载体，所以枚举值直接照标准命名，不另造词。
 *
 * <p>选哪一种由这道工序的物理性质决定——热处理看装炉重量、电镀看受镀面积或挂具位、镀膜看件数——属于主数据配置，
 * 所以这里只列口径，代码里不按行业分支。
 */
@NullMarked
@Getter
public enum AllocationBasisEnum implements BaseEnum {

  /** 按件数：实物量法，各行占用件数占比 */
  QUANTITY("Quantity", "按件数"),

  /** 按重量：装炉重量、投料重量占比，热处理与染色常用 */
  WEIGHT("Weight", "按重量"),

  /** 按面积：受镀面积、投影面积占比，电镀与喷涂常用 */
  AREA("Area", "按面积"),

  /** 按机器工时：各行占用设备的时长占比 */
  MACHINE_HOURS("MachineHours", "按机器工时"),

  /** 按人工工时：制度第36条列的人工工时口径 */
  LABOR_HOURS("LaborHours", "按人工工时"),

  /** 按系数：制度第37条的系数法，难度、规格修正后折算的份数 */
  COEFFICIENT("Coefficient", "按系数");

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
  AllocationBasisEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
