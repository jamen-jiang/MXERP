package com.mxerp.basedata.domain.supplier;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商工厂，负责创建供应商 */
@NullMarked
public class SupplierFactory {

  /** 供应商仓储 */
  private final SupplierRepository supplierRepository;

  /**
   * 构造器注入
   *
   * @param supplierRepository 供应商仓储
   */
  public SupplierFactory(SupplierRepository supplierRepository) {
    this.supplierRepository = supplierRepository;
  }

  /**
   * 创建供应商
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   * @return 供应商聚合
   * @throws DomainException 供应商代码已存在时抛出
   */
  public Supplier create(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    // 校验供应商代码唯一
    if (supplierRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("供应商代码" + code + "已存在!");
    }
    return new Supplier(
        code,
        name,
        shortName,
        address,
        telephone,
        fax,
        email,
        contactPerson,
        taxRateId,
        payMode,
        deliveryMode,
        remark);
  }
}
