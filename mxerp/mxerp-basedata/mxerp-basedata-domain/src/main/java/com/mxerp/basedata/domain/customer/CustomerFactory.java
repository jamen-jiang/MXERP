package com.mxerp.basedata.domain.customer;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 客户工厂，负责创建客户 */
@NullMarked
public class CustomerFactory {

  /** 客户仓储 */
  private final CustomerRepository customerRepository;

  /**
   * 构造器注入
   *
   * @param customerRepository 客户仓储
   */
  public CustomerFactory(CustomerRepository customerRepository) {
    this.customerRepository = customerRepository;
  }

  /**
   * 创建客户
   *
   * @param code 客户代码
   * @param name 客户名称
   * @param shortName 客户简称（可空）
   * @param taxRegisterCode 税务登记号（可空）
   * @param currencyId 结算币别Id
   * @param taxRateId 税率Id（可空）
   * @param phone 电话号码（可空）
   * @param email 邮箱（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 客户聚合
   * @throws DomainException 客户代码已存在时抛出
   */
  public Customer create(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String taxRegisterCode,
      long currencyId,
      @Nullable Long taxRateId,
      @Nullable String phone,
      @Nullable String email,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      boolean disabled,
      @Nullable String remark) {
    // 校验客户代码唯一
    if (customerRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("客户代码" + code + "已存在!");
    }
    return new Customer(
        code,
        name,
        shortName,
        taxRegisterCode,
        currencyId,
        taxRateId,
        phone,
        email,
        country,
        province,
        city,
        district,
        zip,
        address,
        disabled,
        remark);
  }
}
