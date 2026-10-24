package com.mxerp.basedata.domain.customermaterial;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 客户物料工厂，负责创建客户物料 */
@NullMarked
public class CustomerMaterialFactory {

  /** 客户物料仓储 */
  private final CustomerMaterialRepository customerMaterialRepository;

  /**
   * 构造器注入
   *
   * @param customerMaterialRepository 客户物料仓储
   */
  public CustomerMaterialFactory(CustomerMaterialRepository customerMaterialRepository) {
    this.customerMaterialRepository = customerMaterialRepository;
  }

  /**
   * 创建客户物料
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   * @return 客户物料聚合
   * @throws DomainException 同一客户下这个物料已经有对照行时抛出
   */
  public CustomerMaterial create(
      long customerId,
      long materialId,
      @Nullable Long variantId,
      @Nullable String customerProductCode,
      @Nullable String customerProductName) {
    if (customerMaterialRepository.isExistByCustomerIdAndMaterialIdAndVariantIdExcludeId(
        customerId, materialId, variantId)) {
      throw new DomainException("该客户这个物料的对照已经存在!");
    }
    return new CustomerMaterial(
        customerId, materialId, variantId, customerProductCode, customerProductName);
  }
}
