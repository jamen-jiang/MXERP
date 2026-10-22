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
   * @param supplierGroupId 供应商分类Id
   * @param currencyId 结算币别Id
   * @param taxRegisterCode 税务登记号（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 供应商聚合
   * @throws DomainException 供应商代码已存在时抛出
   */
  public Supplier create(
      String code,
      String name,
      @Nullable String shortName,
      long supplierGroupId,
      long currencyId,
      @Nullable String taxRegisterCode,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark) {
    // 校验供应商代码唯一
    if (supplierRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("供应商代码" + code + "已存在!");
    }
    return new Supplier(
        code,
        name,
        shortName,
        supplierGroupId,
        currencyId,
        taxRegisterCode,
        country,
        province,
        city,
        district,
        zip,
        address,
        telephone,
        fax,
        email,
        taxRateId,
        disabled,
        remark);
  }
}
