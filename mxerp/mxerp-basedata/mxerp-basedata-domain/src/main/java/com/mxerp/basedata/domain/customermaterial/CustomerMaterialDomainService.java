package com.mxerp.basedata.domain.customermaterial;

import com.mxerp.basedata.domain.customer.CustomerRepository;
import com.mxerp.basedata.domain.material.MaterialRepository;
import com.mxerp.basedata.domain.variant.Variant;
import com.mxerp.basedata.domain.variant.VariantRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 客户物料领域服务，负责跨聚合编排 */
@NullMarked
public class CustomerMaterialDomainService {

  /** 客户物料工厂 */
  private final CustomerMaterialFactory customerMaterialFactory;

  /** 客户物料仓储 */
  private final CustomerMaterialRepository customerMaterialRepository;

  /** 客户仓储 */
  private final CustomerRepository customerRepository;

  /** 物料仓储 */
  private final MaterialRepository materialRepository;

  /** SKU 仓储 */
  private final VariantRepository variantRepository;

  /**
   * 构造器注入
   *
   * @param customerMaterialFactory 客户物料工厂
   * @param customerMaterialRepository 客户物料仓储
   * @param customerRepository 客户仓储
   * @param materialRepository 物料仓储
   * @param variantRepository SKU 仓储
   */
  public CustomerMaterialDomainService(
      CustomerMaterialFactory customerMaterialFactory,
      CustomerMaterialRepository customerMaterialRepository,
      CustomerRepository customerRepository,
      MaterialRepository materialRepository,
      VariantRepository variantRepository) {
    this.customerMaterialFactory = customerMaterialFactory;
    this.customerMaterialRepository = customerMaterialRepository;
    this.customerRepository = customerRepository;
    this.materialRepository = materialRepository;
    this.variantRepository = variantRepository;
  }

  /**
   * 新建客户物料
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   * @return 客户物料聚合
   * @throws DomainException 客户/物料/SKU/单位不存在、SKU 不属于该物料、或对照已存在时抛出
   */
  public CustomerMaterial create(
      long customerId,
      long materialId,
      @Nullable Long variantId,
      @Nullable String customerProductCode,
      @Nullable String customerProductName) {
    validateReferences(customerId, materialId, variantId);
    return customerMaterialFactory.create(
        customerId, materialId, variantId, customerProductCode, customerProductName);
  }

  /**
   * 修改客户物料
   *
   * @param id 客户物料Id
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   * @return 客户物料聚合
   * @throws DomainException 客户物料不存在时抛出
   */
  public CustomerMaterial modify(
      long id,
      @Nullable String customerProductCode,
      @Nullable String customerProductName) {
    CustomerMaterial model = getExist(id);
    model.modify(customerProductCode, customerProductName);
    return model;
  }

  /**
   * 删除客户物料（逻辑删除）
   *
   * @param id 客户物料Id
   * @return 客户物料聚合（已标记删除）
   * @throws DomainException 客户物料不存在时抛出
   */
  public CustomerMaterial delete(long id) {
    CustomerMaterial model = getExist(id);
    model.delete();
    return model;
  }

  /**
   * 按Id取客户物料
   *
   * @param id 客户物料Id
   * @return 客户物料聚合
   * @throws DomainException 客户物料不存在时抛出
   */
  private CustomerMaterial getExist(long id) {
    CustomerMaterial model = customerMaterialRepository.get(id);
    if (model == null) {
      throw new DomainException("客户物料" + id + "不存在!");
    }
    return model;
  }

  /**
   * 校验引用的主数据
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @throws DomainException 任一引用不存在、或 SKU 不属于该物料时抛出
   */
  private void validateReferences(
      long customerId, long materialId, @Nullable Long variantId) {
    if (materialRepository.get(materialId) == null) {
      throw new DomainException("物料" + materialId + "不存在!");
    }
    if (customerRepository.get(customerId) == null) {
      throw new DomainException("客户" + customerId + "不存在!");
    }
    if (variantId != null) {
      Variant variant = variantRepository.get(variantId);
      if (variant == null) {
        throw new DomainException("SKU" + variantId + "不存在!");
      }
      if (!Objects.equals(variant.getMaterialId(), materialId)) {
        throw new DomainException("SKU" + variantId + "不属于物料" + materialId + "!");
      }
    }
  }
}
