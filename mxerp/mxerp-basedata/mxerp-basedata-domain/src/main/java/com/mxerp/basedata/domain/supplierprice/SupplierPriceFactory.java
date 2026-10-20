package com.mxerp.basedata.domain.supplierprice;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商价格卡工厂，负责创建价格卡 */
@NullMarked
public class SupplierPriceFactory {

  /** 供应商价格卡仓储 */
  private final SupplierPriceRepository supplierPriceRepository;

  /**
   * 构造器注入
   *
   * @param supplierPriceRepository 供应商价格卡仓储
   */
  public SupplierPriceFactory(SupplierPriceRepository supplierPriceRepository) {
    this.supplierPriceRepository = supplierPriceRepository;
  }

  /**
   * 创建价格卡（初始状态为待生效）
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
   * @return 价格卡聚合
   * @throws DomainException 同供应商同 SKU 同生效时间已有卡时抛出
   */
  public SupplierPrice create(
      Long supplierId,
      Long productId,
      Long variantId,
      BigDecimal price,
      Long currencyId,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 同一供应商同一 SKU 的生效时间不能撞车，否则取价会出现两张都合法
    if (supplierPriceRepository.isExistBySupplierIdAndVariantIdAndEffectiveFromExcludeId(
        supplierId, variantId, effectiveFrom)) {
      throw new DomainException("该供应商在此生效时间的价格卡已存在!");
    }
    return new SupplierPrice(
        supplierId,
        productId,
        variantId,
        price,
        currencyId,
        moq,
        effectiveFrom,
        effectiveTo,
        remark);
  }
}
