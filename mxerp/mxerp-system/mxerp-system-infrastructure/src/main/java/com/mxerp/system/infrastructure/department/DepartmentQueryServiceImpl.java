package com.mxerp.system.infrastructure.department;

import com.mxerp.system.application.department.query.DepartmentQueryService;
import com.mxerp.system.application.department.query.DepartmentResponse;
import com.mxerp.system.application.department.query.GetDepartmentsQuery;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.department.Department;
import com.mxerp.system.infrastructure.company.CompanyJpa;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

/** 部门查询服务实现（JPA Specification） */
@NullMarked
@Service
public class DepartmentQueryServiceImpl implements DepartmentQueryService {

  /** 部门 JPA 接口 */
  private final DepartmentJpa departmentJpa;

  /** 公司 JPA 接口 */
  private final CompanyJpa companyJpa;

  /**
   * 构造器注入
   *
   * @param departmentJpa 部门 JPA 接口
   * @param companyJpa 公司 JPA 接口
   */
  public DepartmentQueryServiceImpl(DepartmentJpa departmentJpa, CompanyJpa companyJpa) {
    this.departmentJpa = departmentJpa;
    this.companyJpa = companyJpa;
  }

  /**
   * 按Id查询部门（含公司 / 父部门）
   *
   * @param id 部门Id
   * @return 部门响应，不存在返回 null
   */
  @Override
  @Nullable
  public DepartmentResponse getById(long id) {
    Department model = departmentJpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 查询部门列表（含公司 / 父部门）
   *
   * @param query 查询条件
   * @return 部门响应列表
   */
  @Override
  public List<DepartmentResponse> getList(GetDepartmentsQuery query) {
    Specification<Department> spec = buildSpec(query);
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    return toResponseList(departmentJpa.findAll(spec, sort));
  }

  /**
   * 构造查询条件：公司精确匹配，代码 / 名称模糊匹配
   *
   * @param query 查询条件
   * @return 查询条件
   */
  private Specification<Department> buildSpec(GetDepartmentsQuery query) {
    return (root, criteriaQuery, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      if (query.getCompanyId() != null) {
        predicates.add(cb.equal(root.get("companyId"), query.getCompanyId()));
      }
      if (query.getParentId() != null) {
        predicates.add(cb.equal(root.get("parentId"), query.getParentId()));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 批量转响应，公司和父部门名称用一次查库批量填充（避免 N+1）
   *
   * @param list 部门列表
   * @return 部门响应列表
   */
  private List<DepartmentResponse> toResponseList(List<Department> list) {
    if (list.isEmpty()) {
      return List.of();
    }

    // 公司批量查
    Map<Long, Company> companyMap = new HashMap<>();
    List<Long> companyIds = list.stream().map(Department::getCompanyId).distinct().toList();
    if (!companyIds.isEmpty()) {
      for (Company item : companyJpa.findAllById(companyIds)) {
        companyMap.put(item.getId(), item);
      }
    }

    // 父部门批量查
    Map<Long, Department> parentMap = new HashMap<>();
    List<Long> parentIds =
        list.stream()
            .map(Department::getParentId)
            .filter(Objects::nonNull)
            .distinct()
            .toList();
    if (!parentIds.isEmpty()) {
      for (Department item : departmentJpa.findAllById(parentIds)) {
        parentMap.put(item.getId(), item);
      }
    }

    List<DepartmentResponse> result = new ArrayList<>(list.size());
    for (Department item : list) {
      result.add(toResponse(item, companyMap.get(item.getCompanyId()), parentMap.get(item.getParentId())));
    }
    return result;
  }

  /**
   * 聚合转响应（单个，内部再补公司和父部门）
   *
   * @param model 部门聚合
   * @return 部门响应
   */
  private DepartmentResponse toResponse(Department model) {
    Company company = companyJpa.findById(model.getCompanyId()).orElse(null);
    Department parent =
        model.getParentId() == null ? null : departmentJpa.findById(model.getParentId()).orElse(null);
    return toResponse(model, company, parent);
  }

  /**
   * 聚合转响应
   *
   * @param model 部门聚合
   * @param company 所属公司（可空）
   * @param parent 父部门（可空）
   * @return 部门响应
   */
  private DepartmentResponse toResponse(
      Department model, @Nullable Company company, @Nullable Department parent) {
    DepartmentResponse response = new DepartmentResponse();
    response.setId(model.getId());
    response.setCompanyId(model.getCompanyId());
    response.setCompanyCode(company == null ? "" : company.getCode());
    response.setCompanyName(company == null ? "" : company.getName());
    response.setLevel(model.getLevel());
    response.setParentId(model.getParentId());
    response.setParentCode(parent == null ? null : parent.getCode());
    response.setParentName(parent == null ? null : parent.getName());
    response.setCode(model.getCode());
    response.setName(model.getName());
    response.setTelephone(model.getTelephone());
    response.setFax(model.getFax());
    response.setEmail(model.getEmail());
    response.setSort(model.getSort());
    response.setAddress(model.getAddress());
    response.setRemark(model.getRemark());
    return response;
  }
}
