package com.mxerp.basedata.domain.purchaseprice;

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
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 采购价格（某个供应商在一段时间里卖某个物料多少钱）
 *
 * <p>价格默认挂在物料系列上：供应商通常按毛坯/系列报价，同一系列几百个度数价格一样。
 * {@code variantId} 可空——个别度数有特别价时才填；取价时先用 SKU 那张，没有再回落到系列价。
 *
 * <p>为什么把采购价从物料上搬出来：物料的采购价本来就是「供应商 × 物料」的属性，不是一个标量。
 * 钉在 {@code Material}/{@code Variant} 上只能存一个价，换供应商谈价就得覆盖旧值，历史价当场丢失；
 * 物料主数据也会被采购域的值污染，自制件根本没人买也要挂个价。口径与 SAP 的采购信息记录
 * （物料 × 供应商 × 生效期 × 价格）一致。
 *
 * <p>没有状态列：当前价 = 生效时间已到、失效时间未到的那张。新卡生效时，同键的旧卡在交接点收口；
 * 要提前作废，把失效时间改到当天。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "PurchasePrice",
    indexes = {
      @Index(name = "IX_PurchasePrice_SupplierId", columnList = "SupplierId"),
      @Index(name = "IX_PurchasePrice_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_PurchasePrice_VariantId", columnList = "VariantId"),
      @Index(name = "IX_PurchasePrice_EffectiveFrom", columnList = "EffectiveFrom")
      // 注意：SupplierId + MaterialId + VariantId + EffectiveFrom 的过滤唯一索引带 WHERE
      // IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class PurchasePrice extends UpdateAggregateRoot {

  /** 供应商Id（不可变：换供应商就是另一张卡） */
  @Column(name = "SupplierId", nullable = false)
  private Long supplierId;

  /** 物料系列Id（不可变）：价格默认挂在这一层，同系列的所有度数共用 */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** SKU Id（可空，不可变）：个别度数有特别价时才填 */
  @Column(name = "VariantId")
  @Nullable
  private Long variantId;

  /** 采购价格（不可变：改价请新建一张卡，覆盖历史价等于销毁追溯依据） */
  @Column(name = "Price", nullable = false, precision = 18, scale = 2)
  private BigDecimal price;

  /** 币种Id（跟价走） */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 起订量（可空，这个供应商谈下来的量） */
  @Column(name = "Moq", precision = 18, scale = 2)
  @Nullable
  private BigDecimal moq;

  /** 生效时间 */
  @Column(name = "EffectiveFrom", nullable = false)
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空，为空表示一直有效直到被新卡接替） */
  @Column(name = "EffectiveTo")
  @Nullable
  private LocalDateTime effectiveTo;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，个别度数的特别价）
   * @param price 采购价格
   * @param currencyId 币种Id
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  PurchasePrice(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      BigDecimal price,
      long currencyId,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    this.supplierId = supplierId;
    this.materialId = materialId;
    this.variantId = variantId;
    this.price = validatePrice(price);
    this.currencyId = currencyId;

    apply(moq, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 修改价格卡
   *
   * <p>供应商、物料、SKU 与价格是卡的身份，创建后不改；能改的只有起订量、有效期和备注。
   * 调价请新建一张卡，覆盖历史价等于销毁追溯依据。
   *
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  void modify(
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(moq, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 该时刻是否落在本卡的有效区间内
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
   * 判断生效区间是否与给定区间重叠
   *
   * @param from 待比较区间的生效时间（可空，空视为无起始）
   * @param to 待比较区间的失效时间（可空，空视为无限期）
   * @return true 表示区间重叠
   */
  public boolean overlaps(@Nullable LocalDateTime from, @Nullable LocalDateTime to) {
    if (to != null && !this.effectiveFrom.isBefore(to)) {
      return false;
    }
    return from == null || this.effectiveTo == null || from.isBefore(this.effectiveTo);
  }

  /**
   * 收口生效区间：新卡在交接点接替本卡
   *
   * @param moment 新的失效时间（新卡的生效时间）
   * @throws DomainException 收口时间不晚于本卡生效时间时抛出
   */
  public void closeAt(LocalDateTime moment) {
    if (!moment.isAfter(this.effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }
    this.effectiveTo = moment;
  }

  // ---------- 私有：校验与赋值 ----------

  /**
   * 统一的可变字段赋值入口（构造器与修改共用）
   *
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 失效时间不晚于生效时间、或起订量为负时抛出
   */
  private void apply(
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    if (effectiveTo != null && !effectiveTo.isAfter(effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }
    if (moq != null && moq.signum() < 0) {
      throw new DomainException("起订量不能为负数");
    }

    this.moq = moq;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.remark = remark;
  }

  /**
   * 校验采购价格
   *
   * <p>价格为 0 是合法的（赠品、样品），只挡负数。
   *
   * @param price 采购价格
   * @return 原样返回，便于构造器内联赋值
   * @throws DomainException 价格为负时抛出
   */
  private BigDecimal validatePrice(BigDecimal price) {
    if (price.signum() < 0) {
      throw new DomainException("采购价格不能为负数");
    }
    return price;
  }
}
