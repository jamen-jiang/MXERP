package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司工厂，负责创建公司 */
@NullMarked
public class CompanyFactory {

  /** 公司仓储 */
  private final CompanyRepository companyRepository;

  /**
   * 构造器注入
   *
   * @param companyRepository 公司仓储
   */
  public CompanyFactory(CompanyRepository companyRepository) {
    this.companyRepository = companyRepository;
  }

  /**
   * 创建公司
   *
   * @param code 公司代码
   * @param name 公司名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 公司聚合
   * @throws DomainException 公司代码已存在时抛出
   */
  public Company create(
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
      @Nullable String address,
      @Nullable String billingAddress,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验公司代码唯一
    if (companyRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("公司代码" + code + "已存在!");
    }
    return new Company(
        code,
        name,
        telephone,
        fax,
        email,
        areaCode,
        postCode,
        address,
        billingAddress,
        sort,
        remark);
  }
}
