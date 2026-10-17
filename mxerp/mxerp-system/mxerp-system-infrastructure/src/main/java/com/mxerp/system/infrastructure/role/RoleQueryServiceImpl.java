package com.mxerp.system.infrastructure.role;

import com.mxerp.system.application.role.query.GetPageRolesQuery;
import com.mxerp.system.application.role.query.RoleQueryService;
import com.mxerp.system.application.role.query.RoleResponse;
import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.domain.role.Role;
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

/** 角色查询服务实现（JPA Specification） */
@NullMarked
@Service
public class RoleQueryServiceImpl implements RoleQueryService {

  /** 角色 JPA 接口 */
  private final RoleJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public RoleQueryServiceImpl(RoleJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按Id查询角色
   *
   * @param id 角色Id
   * @return 角色响应，不存在返回 null
   */
  @Override
  @Nullable
  public RoleResponse getById(long id) {
    Role model = jpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 查询所有角色
   *
   * @return 角色响应列表
   */
  @Override
  public List<RoleResponse> getList() {
    Sort sort = Sort.by(Sort.Direction.ASC, "name");
    List<RoleResponse> result = new ArrayList<>();
    for (Role item : jpa.findAll(sort)) {
      result.add(toResponse(item));
    }
    return result;
  }

  /**
   * 分页查询角色
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @Override
  public PageResponse<RoleResponse> getPage(GetPageRolesQuery query) {
    Specification<Role> spec =
        (root, criteriaQuery, cb) -> {
          List<Predicate> predicates = new ArrayList<>();
          if (StringUtils.isNotBlank(query.getName())) {
            predicates.add(cb.like(root.get("name"), "%" + query.getName().trim() + "%"));
          }
          return cb.and(predicates.toArray(new Predicate[0]));
        };
    Sort sort = Sort.by(Sort.Direction.ASC, "name");
    PageRequest pageable =
        PageRequest.of(Math.max(query.getPageIndex() - 1, 0), query.getPageSize(), sort);

    Page<Role> page = jpa.findAll(spec, pageable);
    List<RoleResponse> list = new ArrayList<>(page.getContent().size());
    for (Role item : page.getContent()) {
      list.add(toResponse(item));
    }
    return new PageResponse<>(
        (int) page.getTotalElements(), query.getPageSize(), query.getPageIndex(), list);
  }

  /**
   * 聚合转响应
   *
   * @param model 角色聚合
   * @return 角色响应
   */
  private RoleResponse toResponse(Role model) {
    RoleResponse response = new RoleResponse();
    response.setId(model.getId());
    response.setName(model.getName());
    response.setRemark(model.getRemark());
    return response;
  }
}
