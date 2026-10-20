package com.mxerp.basedata.domain.purchaseprice;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 采购价格仓储接口 */
@NullMarked
public interface PurchasePriceRepository extends Repository<PurchasePrice> {

  /**
   * 按供应商 + 物料系列 + SKU 查询全部价格卡
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，空表示查系列级价格）
   * @return 价格卡列表
   */
  List<PurchasePrice> getListBySupplierIdAndMaterialIdAndVariantId(
      long supplierId, long materialId, @Nullable Long variantId);

  /**
   * 判断同键同生效时间是否已有价格卡
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param effectiveFrom 生效时间
   * @param id 排除的价格卡Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      LocalDateTime effectiveFrom,
      @Nullable Long id);

  /**
   * 判断同键同生效时间是否已有价格卡
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param effectiveFrom 生效时间
   * @return true 表示已存在
   */
  default boolean isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      LocalDateTime effectiveFrom) {
    return isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
        supplierId, materialId, variantId, effectiveFrom, null);
  }
}
