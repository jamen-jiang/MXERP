package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司领域服务，负责跨聚合编排和联系人同步 */
@NullMarked
public class CompanyDomainService {

  /** 公司仓储 */
  private final CompanyRepository companyRepository;

  /**
   * 构造器注入
   *
   * @param companyRepository 公司仓储
   */
  public CompanyDomainService(CompanyRepository companyRepository) {
    this.companyRepository = companyRepository;
  }

  /**
   * 修改公司
   *
   * @param id 公司Id
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
   * @throws DomainException 公司不存在时抛出
   */
  public Company modify(
      long id,
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
    // 校验公司代码唯一（排除自身）
    if (companyRepository.isExistByCodeExcludeId(code, id)) {
      throw new DomainException("公司代码" + code + "已存在!");
    }
    Company model = companyRepository.get(id);
    if (model == null) {
      throw new DomainException("公司不存在!");
    }
    model.modify(
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
    return model;
  }
}
