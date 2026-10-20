package com.mxerp.basedata.domain.supplierprice;

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
 * 供应商价格卡（某个供应商在一段时间里卖某个产品多少钱）
 *
 * <p>为什么把采购价从产品上搬出来：产品的采购价本来就是「供应商 × 产品」的属性，不是一个标量。钉在 {@code Product}/{@code Variant}
 * 上只能存一个价，换供应商谈价就得覆盖旧值，历史价当场丢失；同时产品主数据被采购域的值污染，自制件根本没人买也要挂个价。
 * 搬到这里之后，产品只描述「是什么」，价格随供应商和生效日期走，口径与 SAP 的采购信息记录（物料 × 供应商 × 生效期 × 价格）一致。
 *
 * <p>价格卡挂在 {@code VariantId} 上（辅料也有一条 SKU，能覆盖），同时冗余 {@code ProductId} 便于按产品列出全部价格。
 * 币种跟价走，不跟产品走：从人民币改成美元报价是新建一张卡接替旧卡，而不是把旧卡改成另一个数。
 *
 * <p>生效链与 {@code OperationRate} 同构：业务键是「供应商 + SKU」，新建一律待生效，激活时接替同键的旧卡，
 * 历史卡只用于追溯。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "SupplierPrice",
    indexes = {
      @Index(name = "IX_SupplierPrice_SupplierId", columnList = "SupplierId"),
      @Index(name = "IX_SupplierPrice_ProductId", columnList = "ProductId"),
      @Index(name = "IX_SupplierPrice_VariantId", columnList = "VariantId"),
      @Index(name = "IX_SupplierPrice_EffectiveFrom", columnList = "EffectiveFrom")
      // 注意：SupplierId + VariantId + EffectiveFrom 的过滤唯一索引带 WHERE IsDeleted=0，用 Flyway 创建
    })
public class SupplierPrice extends UpdateAggregateRoot {

  /** 供应商Id（不可变：换供应商就是另一张卡，不然是两个供应商在抢同一条价格记录） */
  @Column(name = "SupplierId", nullable = false)
  private Long supplierId;

  /** 产品Id（不可变，冗余自 Variant，便于按产品列出全部价格） */
  @Column(name = "ProductId", nullable = false)
  private Long productId;

  /** SKU Id（不可变，价格实际挂在这一层） */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 采购价格（不可变：改价请新建一张待生效的卡，覆盖历史价等于销毁追溯依据） */
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
   * 构造器：新建的价格卡一律是待生效，激活由 {@link #activate()} 完成
   *
   * @param supplierId 供应商Id
   * @param productId 产品Id
   * @param variantId SKU Id
   * @param price 采购价格
   * @param currencyId 币种Id
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  SupplierPrice(
      Long supplierId,
      Long productId,
      Long variantId,
      BigDecimal price,
      Long currencyId,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 供应商、产品、SKU、价格、币种属于卡的身份，创建后由 activate/retire 推进状态，不改这些
    this.supplierId = supplierId;
    this.productId = productId;
    this.variantId = variantId;
    this.price = validatePrice(price);
    this.currencyId = currencyId;

    apply(moq, effectiveFrom, effectiveTo, remark);
    this.status = SupplierPriceStatusEnum.PENDING.getCode();
  }

  /**
   * 修改价格卡
   *
   * <p>只改还没生效的卡。当前生效的卡一旦被改，之前按它算过的采购成本就再也对不上了，要调价请新建一张待生效的卡。
   *
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 卡已不是待生效状态时抛出
   */
  void modify(
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    if (!isPending()) {
      throw new DomainException("该供应商价格卡已生效，不能修改，请新建一张待生效的价格卡");
    }
    apply(moq, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 激活：待生效转当前生效
   *
   * <p>同键（供应商 + SKU）的旧卡由领域服务逐个置为历史，本方法只管自己，避免聚合越权改别的卡。
   *
   * @throws DomainException 卡不是待生效状态时抛出
   */
  public void activate() {
    if (!isPending()) {
      throw new DomainException("该供应商价格卡不是待生效状态，不能激活");
    }
    this.status = SupplierPriceStatusEnum.CURRENT.getCode();
  }

  /**
   * 接替：当前生效转历史
   *
   * <p>转历史后取不到价，但采购单下单时已经把价快照进单行，历史单据不受影响。
   *
   * @throws DomainException 卡不是当前生效状态时抛出
   */
  public void retire() {
    if (!isCurrent()) {
      throw new DomainException("该供应商价格卡不是当前生效状态，不能转历史");
    }
    this.status = SupplierPriceStatusEnum.PREVIOUS.getCode();
  }

  // ---------- 计算属性 ----------

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
    return SupplierPriceStatusEnum.PENDING.getCode().equals(this.status);
  }

  /**
   * 是否当前生效
   *
   * @return true 表示当前生效
   */
  public boolean isCurrent() {
    return SupplierPriceStatusEnum.CURRENT.getCode().equals(this.status);
  }

  /**
   * 是否已转历史
   *
   * @return true 表示历史卡
   */
  public boolean isPrevious() {
    return SupplierPriceStatusEnum.PREVIOUS.getCode().equals(this.status);
  }

  // ---------- 私有：校验与赋值 ----------

  /**
   * 统一的可变字段赋值入口（构造器与修改共用）
   *
   * <p>把「价格区间校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 两处各写一遍导致漏改。状态不在这里赋值：
   * 新建恒为待生效，之后只能靠 {@link #activate()} 与 {@link #retire()} 推进，防止外部直接塞一个状态进来
   * 把生效链打乱。
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
