package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司领域服务，负责层级编排 */
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
   * 修改公司（含层级编排）
   *
   * @param id 公司Id
   * @param name 名称
   * @param shortName 简称（可空）
   * @param parentId 上级公司Id（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param taxRegisterCode 税务登记号（可空）
   * @param registryCode 工商注册号（可空）
   * @param establishedDate 成立日期（可空）
   * @param currencyId 默认币别Id
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 需要持久化的聚合列表（自身 + 受影响的子孙）
   * @throws DomainException 同级公司名称已存在、公司不存在、或上级公司是自己/自己的子孙时抛出
   */
  public List<Company> modify(
      long id,
      String name,
      @Nullable String shortName,
      @Nullable Long parentId,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String taxRegisterCode,
      @Nullable String registryCode,
      @Nullable LocalDate establishedDate,
      long currencyId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验同级公司名称唯一（排除自身）
    if (companyRepository.isExistByNameAndParentIdExcludeId(name, parentId, id)) {
      throw new DomainException("同级公司名称" + name + "已存在!");
    }
    Company model = companyRepository.get(id);
    if (model == null) {
      throw new DomainException("公司不存在!");
    }
    Company parent = getParent(parentId);
    validateParent(model, parentId, parent);
    model.modify(
        name,
        shortName,
        parentId,
        HierarchyUtils.resolveLevel(parentId, parent, "上级公司"),
        telephone,
        fax,
        email,
        country,
        province,
        city,
        district,
        zip,
        address,
        taxRegisterCode,
        registryCode,
        establishedDate,
        currencyId,
        disabled,
        sort,
        remark);
    List<Company> list = new ArrayList<>();
    setChildrenLevel(model, list);
    list.addFirst(model);
    return list;
  }

  /**
   * 删除自身及所有子孙公司
   *
   * @param id 公司Id
   * @return 需要持久化的聚合列表（已标记删除）
   */
  public List<Company> removeWithChildren(long id) {
    List<Company> list = companyRepository.getWithChildren(id);
    for (Company item : list) {
      item.delete();
    }
    return list;
  }

  /**
   * 查询上级公司
   *
   * @param parentId 上级公司Id（可空）
   * @return 上级公司，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private Company getParent(@Nullable Long parentId) {
    return parentId == null ? null : companyRepository.get(parentId);
  }

  /**
   * 校验上级公司合法
   *
   * <p>不能是自己、不能是自己的子孙——不拦的话层级路径会绕成一个环，"整棵子树"的查询和删除都会跟着出错。
   * 与 {@code Category}、{@code Department} 同一套规矩。
   *
   * @param model 公司
   * @param parentId 上级公司Id（可空）
   * @param parent 上级公司（可空）
   * @throws DomainException 上级公司是自己或自己的子孙时抛出
   */
  private void validateParent(Company model, @Nullable Long parentId, @Nullable Company parent) {
    if (parentId == null || parent == null) {
      return;
    }
    if (Objects.equals(parentId, model.getId())) {
      throw new DomainException("上级公司不能是自己!");
    }
    boolean isDescendant =
        companyRepository.getWithChildren(model.getId()).stream()
            .anyMatch(item -> Objects.equals(item.getId(), parentId));
    if (isDescendant) {
      throw new DomainException("上级公司不能是自己的子孙公司!");
    }
  }

  /**
   * 递归设置下级公司层级
   *
   * @param model 公司
   * @param list 下级公司收集列表
   */
  private void setChildrenLevel(Company model, List<Company> list) {
    if (model.getId() == 0) {
      return;
    }
    List<Company> children = companyRepository.getListByParentId(model.getId());
    HierarchyUtils.resetChildrenLevel(model, children);
    for (Company child : children) {
      setChildrenLevel(child, list);
      list.add(child);
    }
  }
}
