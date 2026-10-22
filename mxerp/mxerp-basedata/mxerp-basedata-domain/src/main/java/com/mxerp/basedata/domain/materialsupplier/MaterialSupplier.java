package com.mxerp.basedata.domain.materialsupplier;

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
 * 物料供应商（某个供应商在一段时间里，按什么价格、什么单位，供某个物料）
 *
 * <p>命名对着 ERPNext 物料底下的 {@code Item Supplier}（物料供应商）与 Odoo 的
 * {@code product_supplierinfo}（供应商价格表）：一行说清「这个物料跟这个供应商之间怎么买卖」——报价、币种、
 * 采购单位、起订量、有效期，外加供应商产品代码/名称、货期、折扣。Odoo 上还剩排序（{@code sequence}）与
 * 公司（{@code company_id}）没做：我们不做多公司，排序等有列表界面再补。
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
 *
 * <p>采购单位也挂在这一层（{@code unitId}，可空即按物料基本单位）而不是物料上：按什么单位进货本来就随
 * 供应商变——这家按卷报、那家按米报，价格与起订量都得跟着这个单位走。Odoo 19 也是把采购单位从产品表挪到了
 * 供应商价格表（{@code product_supplierinfo}）这条线。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "MaterialSupplier",
    indexes = {
      @Index(name = "IX_MaterialSupplier_SupplierId", columnList = "SupplierId"),
      @Index(name = "IX_MaterialSupplier_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_MaterialSupplier_VariantId", columnList = "VariantId"),
      @Index(name = "IX_MaterialSupplier_EffectiveFrom", columnList = "EffectiveFrom")
      // 注意：SupplierId + MaterialId + VariantId + EffectiveFrom 的过滤唯一索引带 WHERE
      // IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class MaterialSupplier extends UpdateAggregateRoot {

  /** 物料系列Id（不可变）：价格默认挂在这一层，同系列的所有度数共用 */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** SKU Id（可空，不可变）：个别度数有特别价时才填 */
  @Column(name = "VariantId")
  @Nullable
  private Long variantId;

  /** 供应商Id（不可变：换供应商就是另一张卡） */
  @Column(name = "SupplierId", nullable = false)
  private Long supplierId;

  /** 供应商产品代码（可空）：对方对这个物料的编号，对账用，照 Odoo 的 Vendor Product Code */
  @Nationalized
  @Column(name = "SupplierProductCode", length = 100)
  @Nullable
  private String supplierProductCode;

  /** 供应商产品名称（可空）：对方对这个物料的叫法，照 Odoo 的 Vendor Product Name */
  @Nationalized
  @Column(name = "SupplierProductName", length = 200)
  @Nullable
  private String supplierProductName;

  /** 采购价格（不可变：改价请新建一张卡，覆盖历史价等于销毁追溯依据） */
  @Column(name = "Price", nullable = false, precision = 18, scale = 2)
  private BigDecimal price;

  /** 折扣（%，可空，空即无折扣） */
  @Column(name = "Discount", precision = 18, scale = 2)
  @Nullable
  private BigDecimal discount;

  /** 币种Id（跟价走） */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 单位Id（可空，空表示按物料基本单位报价）：这家供应商按哪个单位报价，价格与起订量都以此单位计 */
  @Column(name = "UnitId")
  @Nullable
  private Long unitId;

  /** 起订量（可空，这个供应商谈下来的量） */
  @Column(name = "Moq", precision = 18, scale = 2)
  @Nullable
  private BigDecimal moq;

  /** 货期（天，可空）：下单到到货要几天，照 Odoo 的 Lead Time */
  @Column(name = "LeadTime")
  @Nullable
  private Integer leadTime;

  /** 生效时间 */
  @Column(name = "EffectiveFrom", nullable = false)
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空，为空表示一直有效直到被新卡接替） */
  @Column(name = "EffectiveTo")
  @Nullable
  private LocalDateTime effectiveTo;

  /** 排序（可空）：列表里谁先谁后，照 Odoo 的 sequence */
  @Column(name = "Sequence")
  @Nullable
  private Integer sequence;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，个别度数的特别价）
   * @param supplierId 供应商Id
   * @param supplierProductCode 供应商产品代码（可空）
   * @param supplierProductName 供应商产品名称（可空）
   * @param price 采购价格
   * @param discount 折扣（%，可空）
   * @param currencyId 币种Id
   * @param unitId 单位Id（可空，空表示按物料基本单位报价）
   * @param moq 起订量（可空）
   * @param leadTime 货期（天，可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  MaterialSupplier(
      long materialId,
      @Nullable Long variantId,
      long supplierId,
      @Nullable String supplierProductCode,
      @Nullable String supplierProductName,
      BigDecimal price,
      @Nullable BigDecimal discount,
      long currencyId,
      @Nullable Long unitId,
      @Nullable BigDecimal moq,
      @Nullable Integer leadTime,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable Integer sequence,
      @Nullable String remark) {
    this.materialId = materialId;
    this.variantId = variantId;
    this.supplierId = supplierId;
    this.price = validatePrice(price);
    this.currencyId = currencyId;
    this.unitId = unitId;

    apply(
        supplierProductCode,
        supplierProductName,
        discount,
        moq,
        leadTime,
        effectiveFrom,
        effectiveTo,
        sequence,
        remark);
  }

  /**
   * 修改供应商卡
   *
   * <p>供应商、物料、SKU、价格、币种与采购单位是卡的身份，创建后不改；能改的是供应商产品代码/名称、折扣、
   * 起订量、货期、有效期和备注。调价请新建一张卡，覆盖历史价等于销毁追溯依据。
   *
   * @param supplierProductCode 供应商产品代码（可空）
   * @param supplierProductName 供应商产品名称（可空）
   * @param discount 折扣（%，可空）
   * @param moq 起订量（可空）
   * @param leadTime 货期（天，可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      @Nullable String supplierProductCode,
      @Nullable String supplierProductName,
      @Nullable BigDecimal discount,
      @Nullable BigDecimal moq,
      @Nullable Integer leadTime,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable Integer sequence,
      @Nullable String remark) {
    apply(
        supplierProductCode,
        supplierProductName,
        discount,
        moq,
        leadTime,
        effectiveFrom,
        effectiveTo,
        sequence,
        remark);
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
   * @param supplierProductCode 供应商产品代码（可空）
   * @param supplierProductName 供应商产品名称（可空）
   * @param discount 折扣（%，可空）
   * @param moq 起订量（可空）
   * @param leadTime 货期（天，可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 失效时间不晚于生效时间、起订量/货期为负、或折扣超出 0~100 时抛出
   */
  private void apply(
      @Nullable String supplierProductCode,
      @Nullable String supplierProductName,
      @Nullable BigDecimal discount,
      @Nullable BigDecimal moq,
      @Nullable Integer leadTime,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable Integer sequence,
      @Nullable String remark) {
    if (effectiveTo != null && !effectiveTo.isAfter(effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }
    if (moq != null && moq.signum() < 0) {
      throw new DomainException("起订量不能为负数");
    }
    if (leadTime != null && leadTime < 0) {
      throw new DomainException("货期不能为负数");
    }
    if (discount != null
        && (discount.signum() < 0 || discount.compareTo(BigDecimal.valueOf(100)) > 0)) {
      throw new DomainException("折扣必须在 0 到 100 之间");
    }

    this.supplierProductCode = supplierProductCode;
    this.supplierProductName = supplierProductName;
    this.discount = discount;
    this.moq = moq;
    this.leadTime = leadTime;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.sequence = sequence;
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
