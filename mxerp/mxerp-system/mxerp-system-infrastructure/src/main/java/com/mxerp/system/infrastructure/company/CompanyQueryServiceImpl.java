package com.mxerp.system.infrastructure.company;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.company.query.CompanyContactResponse;
import com.mxerp.system.application.company.query.CompanyQueryService;
import com.mxerp.system.application.company.query.CompanyResponse;
import com.mxerp.system.application.company.query.GetCompaniesQuery;
import com.mxerp.system.application.company.query.GetPageCompaniesQuery;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.company.CompanyContact;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import org.apache.commons.lang3.StringUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

/** 公司查询服务实现（JPA Specification） */
@NullMarked
@Service
public class CompanyQueryServiceImpl implements CompanyQueryService {

  /** 公司 JPA 接口 */
  private final CompanyJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public CompanyQueryServiceImpl(CompanyJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按Id查询公司（含联系人）
   *
   * @param id 公司Id
   * @return 公司响应，不存在返回 null
   */
  @Override
  @Nullable
  public CompanyResponse getById(long id) {
    Company model = jpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 查询公司列表
   *
   * @param query 查询条件
   * @return 公司响应列表
   */
  @Override
  public List<CompanyResponse> getList(GetCompaniesQuery query) {
    Specification<Company> spec = buildSpec(query.getRegion(), query.getGroup());
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    List<Company> list = jpa.findAll(spec, sort);
    List<CompanyResponse> result = new ArrayList<>(list.size());
    for (Company item : list) {
      result.add(toResponse(item));
    }
    return result;
  }

  /**
   * 分页查询公司
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @Override
  public PageResponse<CompanyResponse> getPage(GetPageCompaniesQuery query) {
    Specification<Company> spec =
        buildSpec(query.getRegion(), query.getGroup(), query.getCode(), query.getName());
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    PageRequest pageable =
        PageRequest.of(Math.max(query.getPageIndex() - 1, 0), query.getPageSize(), sort);

    Page<Company> page = jpa.findAll(spec, pageable);
    List<CompanyResponse> list = new ArrayList<>(page.getContent().size());
    for (Company item : page.getContent()) {
      list.add(toResponse(item));
    }
    return new PageResponse<>(
        (int) page.getTotalElements(), query.getPageSize(), query.getPageIndex(), list);
  }

  /**
   * 构造列表查询条件
   *
   * @param region 区域（可空）
   * @param group 组别（可空）
   * @return 查询条件
   */
  private Specification<Company> buildSpec(
      @Nullable String region, @Nullable String group) {
    return buildSpec(region, group, null, null);
  }

  /**
   * 构造查询条件：区域 / 组别精确匹配，代码 / 名称模糊匹配
   *
   * @param region 区域（可空）
   * @param group 组别（可空）
   * @param code 公司代码（可空）
   * @param name 公司名称（可空）
   * @return 查询条件
   */
  private Specification<Company> buildSpec(
      @Nullable String region,
      @Nullable String group,
      @Nullable String code,
      @Nullable String name) {
    return (root, query, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      if (StringUtils.isNotBlank(region)) {
        predicates.add(cb.equal(root.get("region"), region.trim()));
      }
      if (StringUtils.isNotBlank(group)) {
        predicates.add(cb.equal(root.get("group"), group.trim()));
      }
      if (StringUtils.isNotBlank(code)) {
        predicates.add(cb.like(root.get("code"), "%" + code.trim() + "%"));
      }
      if (StringUtils.isNotBlank(name)) {
        predicates.add(cb.like(root.get("name"), "%" + name.trim() + "%"));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 聚合转响应
   *
   * @param model 公司聚合
   * @return 公司响应
   */
  private CompanyResponse toResponse(Company model) {
    CompanyResponse response = new CompanyResponse();
    response.setId(model.getId());
    response.setRegion(model.getRegion());
    response.setGroup(model.getGroup());
    response.setCode(model.getCode());
    response.setName(model.getName());
    response.setTelephone(model.getTelephone());
    response.setFax(model.getFax());
    response.setEmail(model.getEmail());
    response.setAreaCode(model.getAreaCode());
    response.setPostCode(model.getPostCode());
    response.setCurrencyId(model.getCurrencyId());
    response.setMarkUp(model.getMarkUp());
    response.setAddress(model.getAddress());
    response.setBillingAddress(model.getBillingAddress());
    response.setSort(model.getSort());
    response.setRemark(model.getRemark());

    List<CompanyContactResponse> contacts = new ArrayList<>(model.getCompanyContacts().size());
    for (CompanyContact item : model.getCompanyContacts()) {
      contacts.add(toContactResponse(model.getId(), item));
    }
    response.setCompanyContacts(contacts);
    return response;
  }

  /**
   * 联系人实体转响应
   *
   * @param companyId 公司Id（CompanyContact.companyId 由 JoinColumn 维护，需手动回填）
   * @param model 联系人实体
   * @return 联系人响应
   */
  private CompanyContactResponse toContactResponse(Long companyId, CompanyContact model) {
    CompanyContactResponse response = new CompanyContactResponse();
    response.setId(model.getId());
    response.setCompanyId(companyId);
    response.setName(model.getName());
    response.setTelephone(model.getTelephone());
    response.setArea(model.getArea());
    response.setAddress(model.getAddress());
    return response;
  }
}
