package com.mxerp.basedata.domain.purchaseprice;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 采购价格工厂，负责创建价格卡 */
@NullMarked
public class PurchasePriceFactory {

  /** 采购价格仓储 */
  private final PurchasePriceRepository purchasePriceRepository;

  /**
   * 构造器注入
   *
   * @param purchasePriceRepository 采购价格仓储
   */
  public PurchasePriceFactory(PurchasePriceRepository purchasePriceRepository) {
    this.purchasePriceRepository = purchasePriceRepository;
  }

  /**
   * 创建价格卡
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
   * @return 价格卡聚合
   * @throws DomainException 同键同生效时间已有卡时抛出
   */
  public PurchasePrice create(
      Long supplierId,
      Long materialId,
      @Nullable Long variantId,
      BigDecimal price,
      Long currencyId,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 同键（供应商 + 物料 + SKU）的生效时间不能撞车，否则取价会出现两张都合法
    if (purchasePriceRepository
        .isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
            supplierId, materialId, variantId, effectiveFrom)) {
      throw new DomainException("该供应商这个物料在此生效时间的价格已存在!");
    }
    return new PurchasePrice(
        supplierId,
        materialId,
        variantId,
        price,
        currencyId,
        moq,
        effectiveFrom,
        effectiveTo,
        remark);
  }
}
