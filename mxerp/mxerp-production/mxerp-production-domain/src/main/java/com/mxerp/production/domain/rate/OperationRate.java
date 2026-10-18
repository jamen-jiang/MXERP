package com.mxerp.production.domain.rate;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工序费率卡（一道工序在一段时间里怎么计价）
 *
 * <p>为什么把费率从路线行里搬出来：镜片的工艺一年改不了几次，但电价、人工、外协谈价随时在变。费率留在路线上，
 * 调一次价就得发布一条新路线，所有产品跟着升版；搬到这里之后，路线只长期引用费率卡的代码，改价变成「多一张卡」，
 * 路线一个字节都不用动。口径与 Dynamics 365 的 Cost category price 一致：按生效日期接力，历史卡只用于追溯。
 *
 * <p>三块计价并存：准备与加工按小时（工时字段是分钟，用到时要除以 60），计件按产出（元/件）。不需要的填 0，
 * 因此不需要一个互斥的「计价方式」枚举。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "OperationRate",
    indexes = {
      @Index(name = "IX_OperationRate_Code", columnList = "Code"),
      @Index(name = "IX_OperationRate_Name", columnList = "Name"),
      @Index(name = "IX_OperationRate_Status", columnList = "Status"),
      @Index(name = "IX_OperationRate_EffectiveFrom", columnList = "EffectiveFrom")
      // 注意：Code + EffectiveFrom 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class OperationRate extends UpdateAggregateRoot {

  /** 费率卡代码：同一代码下的多张卡按生效日期接力，路线长期引用的就是这个代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 费率卡名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 准备费率（元/小时，0 表示不计备料成本） */
  @Column(name = "SetupRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal setupRate;

  /** 加工费率（元/小时，0 表示不计加工成本） */
  @Column(name = "RunRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal runRate;

  /** 计件费率（元/件，0 表示不按产出计价） */
  @Column(name = "QuantityRate", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantityRate;

  /** 生效时间 */
  @Column(name = "EffectiveFrom", nullable = false)
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空，为空表示一直有效直到被新卡接替） */
  @Column(name = "EffectiveTo")
  @Nullable
  private LocalDateTime effectiveTo;

  /** 状态：待生效、当前生效、历史 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器：新建的费率卡一律是待生效，激活由 {@link #activate()} 完成
   *
   * @param code 费率卡代码
   * @param name 费率卡名称
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  OperationRate(
      String code,
      String name,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(
        code,
        name,
        setupRate,
        runRate,
        quantityRate,
        effectiveFrom,
        effectiveTo,
        remark);
    this.status = OperationRateStatusEnum.PENDING.getCode();
  }

  /**
   * 修改费率卡
   *
   * <p>只改还没生效的卡。当前生效的卡一旦被改，之前按它算过的成本就再也对不上了，要调价请新建一张待生效的卡。
   *
   * @param code 费率卡代码
   * @param name 费率卡名称
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 卡已不是待生效状态时抛出
   */
  void modify(
      String code,
      String name,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    if (!isPending()) {
      throw new DomainException("费率卡" + this.code + "已生效，不能修改，请新建一张待生效的费率卡");
    }
    apply(code, name, setupRate, runRate, quantityRate, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 激活：待生效转当前生效
   *
   * <p>同代码下的旧卡由领域服务逐个置为历史，本方法只管自己，避免聚合越权改别的卡。
   *
   * @throws DomainException 卡不是待生效状态时抛出
   */
  public void activate() {
    if (!isPending()) {
      throw new DomainException("费率卡" + this.code + "不是待生效状态，不能激活");
    }
    this.status = OperationRateStatusEnum.CURRENT.getCode();
  }

  /**
   * 接替：当前生效转历史
   *
   * <p>转历史后取不到价，但工单下达时已经把费率快照进工序行，在制工单与历史成本都不受影响。
   *
   * @throws DomainException 卡不是当前生效状态时抛出
   */
  public void retire() {
    if (!isCurrent()) {
      throw new DomainException("费率卡" + this.code + "不是当前生效状态，不能转历史");
    }
    this.status = OperationRateStatusEnum.PREVIOUS.getCode();
  }

  /**
   * 该时刻是否落在本卡的有效区间内
   *
   * <p>只看日期，不看状态——激活时点可能滞后于生效时点，靠状态过滤会把该拿的那张价漏掉。
   *
   * @param moment 时刻
   * @return true 表示落在区间内
   */
  public boolean covers(LocalDateTime moment) {
    if (effectiveFrom.isAfter(moment)) {
      return false;
    }
    return this.effectiveTo == null || moment.isBefore(this.effectiveTo);
  }

  /**
   * 是否待生效
   *
   * @return true 表示待生效
   */
  public boolean isPending() {
    return OperationRateStatusEnum.PENDING.getCode().equals(this.status);
  }

  /**
   * 是否当前生效
   *
   * @return true 表示当前生效
   */
  public boolean isCurrent() {
    return OperationRateStatusEnum.CURRENT.getCode().equals(this.status);
  }

  /**
   * 是否已转历史
   *
   * @return true 表示历史卡
   */
  public boolean isPrevious() {
    return OperationRateStatusEnum.PREVIOUS.getCode().equals(this.status);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 费率非负校验 → 生效区间校验 → 赋值」收敛到一处，避免构造器和 {@link #modify}
   * 各写一遍导致规则漂移。状态不在这里赋值：新建恒为待生效，之后只能靠 {@link #activate()} 与 {@link #retire()}
   * 推进，防止外部直接塞一个状态进来把生效链打乱。
   *
   * @param code 费率卡代码
   * @param name 费率卡名称
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空、费率为负、失效时间不晚于生效时间时抛出
   */
  private void apply(
      String code,
      String name,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("费率卡代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("费率卡名称不能为空");
    }
    validateNonNegative("准备费率", setupRate);
    validateNonNegative("加工费率", runRate);
    validateNonNegative("计件费率", quantityRate);
    if (effectiveTo != null && !effectiveTo.isAfter(effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.setupRate = setupRate;
    this.runRate = runRate;
    this.quantityRate = quantityRate;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.remark = remark;
  }

  /**
   * 校验数值字段非负
   *
   * <p>参数类型在 {@code @NullMarked} 下即非空契约，调用方必须保证不为 {@code null}，因此这里只判负数。
   *
   * @param field 字段中文名，用于拼装错误提示
   * @param value 待校验的数值
   * @throws DomainException 值为负数时抛出
   */
  private void validateNonNegative(String field, BigDecimal value) {
    if (value.signum() < 0) {
      throw new DomainException(field + "不能为负数");
    }
  }
}
