package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.math.RoundingMode;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 单位
 *
 * <p>单位属于某个 {@link UnitGroup}（量纲），组内只有一个「基准单位」（{@code isBase}），其余单位用
 * 一个 {@code factor} 表示它与基准的关系：**1 个本单位 = factor 个基准单位**（照 Odoo {@code uom.uom.factor}
 * 的定义：{@code 1 * <this unit> = <factor> * <reference unit>}）。米是「长度」组的基准时，公里填
 * {@code 1000}、厘米填 {@code 0.01}；基准单位自己恒为 {@code 1}。换算只允许在组内做：米与厘米能换，
 * 米与公斤没有入口。
 *
 * <p>{@code rounding} 是数量的落数步长（照 Odoo {@code uom.uom.rounding}）：算出来的数量必须是它的整数倍，
 * 不可再分的「件」填 {@code 1}，按 0.01kg 计量的填 {@code 0.01}。舍入方向不存主数据，
 * 统一按 Odoo 的默认值进位（{@code rounding_method='UP'}），所以这里没有精度小数位和舍入类型两个字段。
 *
 * <p>换算只在这个组里做，跨组（如长度↔重量）不做：照 Odoo 19，物料只按一个单位记账，长度和重量之间不设换算。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Unit",
    indexes = {
      @Index(name = "IX_Unit_GroupId", columnList = "GroupId"),
      @Index(name = "IX_Unit_Code", columnList = "Code"),
      @Index(name = "IX_Unit_Name", columnList = "Name"),
      @Index(name = "IX_Unit_Sequence", columnList = "Sequence")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
      // 注意：GroupId 的过滤唯一索引（基准单位只能一个）带 WHERE IsDeleted=0 AND IsBase=1，用 Flyway 创建
    })
public class Unit extends UpdateAggregateRoot {

  /** 中间计算保留的小数位 */
  private static final int CALC_SCALE = 12;

  /** 落数前归一「数量 ÷ 步长」的小数位，用来吃掉换算率的末位误差 */
  private static final int SNAP_SCALE = 6;

  /** 所属单位组Id */
  @Column(name = "GroupId", nullable = false)
  private Long groupId;

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 换算率：1 个本单位 = factor 个组内基准单位；基准单位恒为 1 */
  @Column(name = "Factor", nullable = false, precision = 28, scale = 12)
  private BigDecimal factor;

  /** 是否组内基准单位 */
  @Column(name = "IsBase", nullable = false)
  private Boolean isBase;

  /** 数量落数步长：换算结果必须是它的整数倍 */
  @Column(name = "Rounding", nullable = false, precision = 28, scale = 12)
  private BigDecimal rounding;

  /** 排序（可空） */
  @Column(name = "Sequence")
  @Nullable
  private Integer sequence;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param groupId 所属单位组Id
   * @param code 代码
   * @param name 名称
   * @param factor 换算率（1 本单位 = factor 基准单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空、换算率或步长不为正数、基准单位换算率不是 1 时抛出
   */
  Unit(
      long groupId,
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    if (groupId <= 0) {
      throw new DomainException("单位组不能为空");
    }
    this.groupId = groupId;
    apply(code, name, factor, isBase, rounding, disabled, sequence, remark);
  }

  /**
   * 修改单位
   *
   * <p>所属单位组创建后不允许修改：换组等于换量纲，历史单据上的单位快照全得跟着重算。
   *
   * @param code 代码
   * @param name 名称
   * @param factor 换算率（1 本单位 = factor 基准单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    apply(code, name, factor, isBase, rounding, disabled, sequence, remark);
  }

  /**
   * 换算成组内另一个单位
   *
   * <p>算法：{@code 数量 × 本单位换算率 ÷ 目标单位换算率}（两个率都是"折成基准单位"，先折到基准再折出去），
   * 中间保留 12 位，最后按**目标单位**的步长落数。两组不同直接报错，不给「米换公斤」留口子。
   *
   * @param target 目标单位
   * @param qty 原数量
   * @return 换算后的数量
   * @throws DomainException 两个单位不在同一组、或数量为负时抛出
   */
  public BigDecimal convertTo(Unit target, BigDecimal qty) {
    if (!this.groupId.equals(target.getGroupId())) {
      throw new DomainException(
          "单位" + this.code + "与" + target.getCode() + "不在同一个单位组，不能换算");
    }
    if (qty.signum() < 0) {
      throw new DomainException("换算数量不能是负数，当前" + qty);
    }
    BigDecimal amount =
        qty
            .multiply(this.factor)
            .divide(target.getFactor(), CALC_SCALE, RoundingMode.HALF_UP);
    return target.roundQty(amount);
  }

  /**
   * 按本单位的步长给数量落数
   *
   * <p>单据上的数量字段、跨组换算的结果都走这里，不要在调用方自己 {@code setScale}。
   * 方向固定进位（Odoo 的 {@code rounding_method='UP'} 默认值）。
   *
   * <p>先把「数量 ÷ 步长」在 6 位小数上四舍五入再进位：换算率是小数，{@code 3根 = 1公斤} 只能存成
   * 0.333333333333，3 根折成公斤会得到 0.999999999999，直接进位就成了 1.01。
   *
   * @param qty 数量
   * @return 落数后的数量
   */
  public BigDecimal roundQty(BigDecimal qty) {
    BigDecimal steps =
        qty
            .divide(this.rounding, SNAP_SCALE, RoundingMode.HALF_UP)
            .setScale(0, RoundingMode.UP);
    return steps.multiply(this.rounding);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param code 代码
   * @param name 名称
   * @param factor 换算率
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空、换算率或步长不为正数、基准单位换算率不是 1 时抛出
   */
  private void apply(
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("单位代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("单位名称不能为空");
    }
    if (factor == null || factor.signum() <= 0) {
      throw new DomainException("单位" + trimmedCode + "的换算率必须大于零");
    }
    if (isBase && factor.compareTo(BigDecimal.ONE) != 0) {
      throw new DomainException("单位" + trimmedCode + "是基准单位，换算率必须是 1");
    }
    if (rounding == null || rounding.signum() <= 0) {
      throw new DomainException("单位" + trimmedCode + "的落数步长必须大于零");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.factor = factor;
    this.isBase = isBase;
    this.rounding = rounding;
    this.disabled = disabled;
    this.sequence = sequence;
    this.remark = remark;
  }
}
