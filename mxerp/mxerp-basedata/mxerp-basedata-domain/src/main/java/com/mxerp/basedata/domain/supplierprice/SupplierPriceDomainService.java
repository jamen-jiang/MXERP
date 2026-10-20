package com.mxerp.basedata.domain.supplierprice;

import com.mxerp.basedata.domain.variant.Variant;
import com.mxerp.basedata.domain.variant.VariantRepository;
import com.mxerp.basedata.domain.supplier.SupplierRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 供应商价格卡领域服务，负责跨聚合编排
 *
 * <p>这里没有 {@code importGenerate}：一个供应商一个 SKU 天生对应多张卡，按业务键查不到唯一一条，
 * 批量导入只能走「新建一张待生效的卡」。
 *
 * <p>入参只要 {@code variantId}，产品Id 由 SKU 反查得到——调用方传不了一个跟 SKU 对不上的 ProductId，
 * 冗余字段就不会写脏。
 */
@NullMarked
public class SupplierPriceDomainService {

  /** 供应商价格卡工厂 */
  private final SupplierPriceFactory supplierPriceFactory;

  /** 供应商价格卡仓储 */
  private final SupplierPriceRepository supplierPriceRepository;

  /** SKU 仓储 */
  private final VariantRepository variantRepository;

  /** 供应商仓储 */
  private final SupplierRepository supplierRepository;

  /**
   * 构造器注入
   *
   * @param supplierPriceFactory 供应商价格卡工厂
   * @param supplierPriceRepository 供应商价格卡仓储
   * @param variantRepository SKU 仓储
   * @param supplierRepository 供应商仓储
   */
  public SupplierPriceDomainService(
      SupplierPriceFactory supplierPriceFactory,
      SupplierPriceRepository supplierPriceRepository,
      VariantRepository variantRepository,
      SupplierRepository supplierRepository) {
    this.supplierPriceFactory = supplierPriceFactory;
    this.supplierPriceRepository = supplierPriceRepository;
    this.variantRepository = variantRepository;
    this.supplierRepository = supplierRepository;
  }

  /**
   * 新建价格卡（待生效）
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @param price 采购价格
   * @param currencyId 币种Id
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 价格卡聚合
   * @throws DomainException 供应商或 SKU 不存在时抛出
   */
  public SupplierPrice create(
      long supplierId,
      long variantId,
      BigDecimal price,
      long currencyId,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    return supplierPriceFactory.create(
        supplierId,
        resolveProductId(supplierId, variantId),
        variantId,
        price,
        currencyId,
        moq,
        effectiveFrom,
        effectiveTo,
        remark);
  }

  /**
   * 修改价格卡（只改还没生效的卡）
   *
   * @param id 价格卡Id
   * @param moq 起订量（可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 价格卡聚合
   * @throws DomainException 价格卡不存在、同键同生效时间已被占用、或卡已生效不可修改时抛出
   */
  public SupplierPrice modify(
      long id,
      @Nullable BigDecimal moq,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    SupplierPrice model = getExist(id);
    if (supplierPriceRepository.isExistBySupplierIdAndVariantIdAndEffectiveFromExcludeId(
        model.getSupplierId(), model.getVariantId(), effectiveFrom, model.getId())) {
      throw new DomainException("该供应商在此生效时间的价格卡已存在!");
    }
    model.modify(moq, effectiveFrom, effectiveTo, remark);
    return model;
  }

  /**
   * 激活价格卡：本卡转当前生效，同供应商同 SKU 的旧卡全部转历史
   *
   * <p>先接替旧卡再激活本卡，中间出异常时两条都不动，不会留下两张当前生效。
   *
   * @param id 价格卡Id
   * @param now 激活时点，必须已落在本卡的有效区间内
   * @return 价格卡聚合
   * @throws DomainException 价格卡不存在、未到生效时间、或本卡不是待生效状态时抛出
   */
  public SupplierPrice activate(long id, LocalDateTime now) {
    SupplierPrice model = getExist(id);
    if (!model.covers(now)) {
      throw new DomainException("价格卡未到生效时间，不能激活");
    }
    List<SupplierPrice> outgoing =
        supplierPriceRepository.getListByVariantId(model.getVariantId()).stream()
            .filter(card -> card.getSupplierId().equals(model.getSupplierId()))
            .filter(SupplierPrice::isCurrent)
            .toList();
    for (SupplierPrice previous : outgoing) {
      previous.retire();
    }
    model.activate();
    return model;
  }

  /**
   * 让价格卡提前退位（转历史）
   *
   * <p>用于报价作废：谈崩了来不及等生效期，直接置历史，之后取价取不到它。
   *
   * @param id 价格卡Id
   * @return 价格卡聚合
   * @throws DomainException 价格卡不存在、或不是当前生效状态时抛出
   */
  public SupplierPrice retire(long id) {
    SupplierPrice model = getExist(id);
    model.retire();
    return model;
  }

  // ==================== 私有：取数与校验 ====================

  /**
   * 校验供应商与 SKU 存在，并回带 SKU 所属的产品Id
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @return 产品Id
   * @throws DomainException 供应商或 SKU 不存在时抛出
   */
  private Long resolveProductId(Long supplierId, Long variantId) {
    if (supplierRepository.get(supplierId) == null) {
      throw new DomainException("供应商" + supplierId + "不存在!");
    }
    Variant variant = variantRepository.get(variantId);
    if (variant == null) {
      throw new DomainException("SKU" + variantId + "不存在!");
    }
    return variant.getProductId();
  }

  /**
   * 取价格卡，不存在则抛异常
   *
   * @param id 价格卡Id
   * @return 价格卡聚合
   * @throws DomainException 价格卡不存在时抛出
   */
  private SupplierPrice getExist(long id) {
    SupplierPrice model = supplierPriceRepository.get(id);
    if (model == null) {
      throw new DomainException("价格卡" + id + "不存在!");
    }
    return model;
  }
}
