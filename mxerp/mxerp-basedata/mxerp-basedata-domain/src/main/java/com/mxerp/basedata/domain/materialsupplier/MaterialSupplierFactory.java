package com.mxerp.basedata.domain.materialsupplier;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 物料供应商工厂，负责创建供应商卡 */
@NullMarked
public class MaterialSupplierFactory {

  /** 物料供应商仓储 */
  private final MaterialSupplierRepository materialSupplierRepository;

  /**
   * 构造器注入
   *
   * @param materialSupplierRepository 物料供应商仓储
   */
  public MaterialSupplierFactory(MaterialSupplierRepository materialSupplierRepository) {
    this.materialSupplierRepository = materialSupplierRepository;
  }

  /**
   * 创建供应商卡
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
   * @return 供应商卡聚合
   * @throws DomainException 同键同生效时间已有卡时抛出
   */
  public MaterialSupplier create(
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
    // 同键（供应商 + 物料 + SKU）的生效时间不能撞车，否则取价会出现两张都合法
    if (materialSupplierRepository
        .isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
            supplierId, materialId, variantId, effectiveFrom)) {
      throw new DomainException("该供应商这个物料在此生效时间的价格已存在!");
    }
    return new MaterialSupplier(
        materialId,
        variantId,
        supplierId,
        supplierProductCode,
        supplierProductName,
        price,
        discount,
        currencyId,
        unitId,
        moq,
        leadTime,
        effectiveFrom,
        effectiveTo,
        sequence,
        remark);
  }
}
