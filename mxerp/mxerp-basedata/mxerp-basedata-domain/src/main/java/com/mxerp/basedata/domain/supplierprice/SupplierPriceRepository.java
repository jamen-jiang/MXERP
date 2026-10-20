package com.mxerp.basedata.domain.supplierprice;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商价格卡仓储接口 */
@NullMarked
public interface SupplierPriceRepository extends Repository<SupplierPrice> {

  /**
   * 取某供应商某 SKU 当前生效的那张价格卡
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @return 价格卡（可空），没有当前生效版本返回 null
   */
  @Nullable SupplierPrice getCurrentBySupplierIdAndVariantId(long supplierId, long variantId);

  /**
   * 取某供应商某 SKU 在指定时刻生效的那张价格卡
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @param moment 时刻
   * @return 价格卡（可空），区间内没有卡返回 null
   */
  @Nullable SupplierPrice getEffectiveBySupplierIdAndVariantIdAt(
      long supplierId, long variantId, LocalDateTime moment);

  /**
   * 按 SKU 查询全部价格卡（含历史，按生效时间倒序）
   *
   * @param variantId SKU Id
   * @return 价格卡列表
   */
  List<SupplierPrice> getListByVariantId(long variantId);

  /**
   * 判断供应商 + SKU + 生效时间是否已被占用
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @param effectiveFrom 生效时间
   * @param id 排除的价格卡Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistBySupplierIdAndVariantIdAndEffectiveFromExcludeId(
      long supplierId, long variantId, LocalDateTime effectiveFrom, @Nullable Long id);

  /**
   * 判断供应商 + SKU + 生效时间是否已被占用
   *
   * @param supplierId 供应商Id
   * @param variantId SKU Id
   * @param effectiveFrom 生效时间
   * @return true 表示已存在
   */
  default boolean isExistBySupplierIdAndVariantIdAndEffectiveFromExcludeId(
      long supplierId, long variantId, LocalDateTime effectiveFrom) {
    return isExistBySupplierIdAndVariantIdAndEffectiveFromExcludeId(
        supplierId, variantId, effectiveFrom, null);
  }
}
