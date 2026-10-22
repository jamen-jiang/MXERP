package com.mxerp.basedata.domain.materialsupplier;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 物料供应商仓储接口 */
@NullMarked
public interface MaterialSupplierRepository extends Repository<MaterialSupplier> {

  /**
   * 按供应商 + 物料系列 + SKU 查询全部供应商卡
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，空表示查系列级价格）
   * @return 供应商卡列表
   */
  List<MaterialSupplier> getListBySupplierIdAndMaterialIdAndVariantId(
      long supplierId, long materialId, @Nullable Long variantId);

  /**
   * 判断同键同生效时间是否已有供应商卡
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param effectiveFrom 生效时间
   * @param id 排除的供应商卡Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistBySupplierIdAndMaterialIdAndVariantIdAndEffectiveFromExcludeId(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      LocalDateTime effectiveFrom,
      @Nullable Long id);

  /**
   * 判断同键同生效时间是否已有供应商卡
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
