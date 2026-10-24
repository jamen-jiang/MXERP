package com.mxerp.basedata.domain.customermaterial;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 客户物料仓储接口 */
@NullMarked
public interface CustomerMaterialRepository extends Repository<CustomerMaterial> {

  /**
   * 按客户 + 物料系列 + SKU 查询
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，空表示查系列级）
   * @return 客户物料列表
   */
  List<CustomerMaterial> getListByCustomerIdAndMaterialIdAndVariantId(
      long customerId, long materialId, @Nullable Long variantId);

  /**
   * 判断同键是否已有客户物料（排除指定 Id，用于修改时排除自身）
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param id 排除的Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCustomerIdAndMaterialIdAndVariantIdExcludeId(
      long customerId, long materialId, @Nullable Long variantId, @Nullable Long id);

  /**
   * 判断同键是否已有客户物料
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @return true 表示已存在
   */
  default boolean isExistByCustomerIdAndMaterialIdAndVariantIdExcludeId(
      long customerId, long materialId, @Nullable Long variantId) {
    return isExistByCustomerIdAndMaterialIdAndVariantIdExcludeId(
        customerId, materialId, variantId, null);
  }
}
