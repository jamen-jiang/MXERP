package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
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
   * @param region 区域
   * @param group 组别（可空）
   * @param code 公司代码
   * @param name 公司名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param currencyId 币种Id
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
      String region,
      @Nullable String group,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
      Long currencyId,
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
        region,
        group,
        code,
        name,
        telephone,
        fax,
        email,
        areaCode,
        postCode,
        currencyId,
        address,
        billingAddress,
        sort,
        remark);
    return model;
  }

  /**
   * 同步联系人列表：按姓名比对，删除未出现的、更新已存在的、添加新的
   *
   * @param model 公司聚合
   * @param list 目标联系人列表（可空）
   */
  public void syncCompanyContacts(Company model, @Nullable List<CompanyContactData> list) {
    List<CompanyContactData> target = list == null ? List.of() : list;

    // 1. 标记删除未在目标列表出现的联系人（按姓名匹配）
    List<CompanyContact> toRemoves =
        model.getCompanyContacts().stream()
            .filter(c -> target.stream().noneMatch(i -> Objects.equals(i.name(), c.getName())))
            .toList();
    for (CompanyContact item : toRemoves) {
      model.deleteContact(item.getId());
    }

    // 2. 建立"姓名 → 已存在联系人Id"的映射
    Map<String, Long> existingDict =
        model.getCompanyContacts().stream()
            .collect(Collectors.toMap(CompanyContact::getName, CompanyContact::getId, (a, b) -> a));

    // 3. 已存在的更新，不存在的添加
    for (CompanyContactData item : target) {
      Long id = existingDict.get(item.name());
      if (id != null) {
        model.modifyContact(id, item.telephone(), item.area(), item.address());
      } else {
        model.addContact(item.name(), item.telephone(), item.area(), item.address());
      }
    }
  }
}
