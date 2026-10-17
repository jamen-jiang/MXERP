package com.mxerp.system.infrastructure.post;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.post.query.GetPagePostsQuery;
import com.mxerp.system.application.post.query.PostQueryService;
import com.mxerp.system.application.post.query.PostResponse;
import com.mxerp.system.application.role.query.RoleResponse;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.department.Department;
import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.Post_Role;
import com.mxerp.system.domain.rank.Rank;
import com.mxerp.system.domain.role.Role;
import com.mxerp.system.infrastructure.company.CompanyJpa;
import com.mxerp.system.infrastructure.department.DepartmentJpa;
import com.mxerp.system.infrastructure.rank.RankJpa;
import com.mxerp.system.infrastructure.role.RoleJpa;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import org.apache.commons.lang3.StringUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

/**
 * 岗位查询服务实现（JPA Specification）
 *
 * <p>岗位本身只存 departmentId / rankId，公司、部门、职级、角色的名称都要跨聚合取，统一用「先批量查
 * 齐 Id 集合、再逐个命中缓存」的方式填充，避免 N+1。
 */
@NullMarked
@Service
public class PostQueryServiceImpl implements PostQueryService {

  /** 岗位 JPA 接口 */
  private final PostJpa postJpa;

  /** 部门 JPA 接口 */
  private final DepartmentJpa departmentJpa;

  /** 职级 JPA 接口 */
  private final RankJpa rankJpa;

  /** 角色 JPA 接口 */
  private final RoleJpa roleJpa;

  /** 公司 JPA 接口 */
  private final CompanyJpa companyJpa;

  /**
   * 构造器注入
   *
   * @param postJpa 岗位 JPA 接口
   * @param departmentJpa 部门 JPA 接口
   * @param rankJpa 职级 JPA 接口
   * @param roleJpa 角色 JPA 接口
   * @param companyJpa 公司 JPA 接口
   */
  public PostQueryServiceImpl(
      PostJpa postJpa,
      DepartmentJpa departmentJpa,
      RankJpa rankJpa,
      RoleJpa roleJpa,
      CompanyJpa companyJpa) {
    this.postJpa = postJpa;
    this.departmentJpa = departmentJpa;
    this.rankJpa = rankJpa;
    this.roleJpa = roleJpa;
    this.companyJpa = companyJpa;
  }

  /**
   * 按Id查询岗位
   *
   * @param id 岗位Id
   * @return 岗位响应，不存在返回 null
   */
  @Override
  @Nullable
  public PostResponse getById(long id) {
    Post model = postJpa.findById(id).orElse(null);
    if (model == null) {
      return null;
    }
    return toResponse(model, new LookupCache(List.of(model)));
  }

  /**
   * 查询所有岗位
   *
   * @return 岗位响应列表
   */
  @Override
  public List<PostResponse> getList() {
    Sort sort = Sort.by(Sort.Direction.ASC, "name");
    return toResponseList(postJpa.findAll(sort));
  }

  /**
   * 分页查询岗位
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @Override
  public PageResponse<PostResponse> getPage(GetPagePostsQuery query) {
    Specification<Post> spec = buildSpec(query);
    Sort sort = Sort.by(Sort.Direction.ASC, "name");
    PageRequest pageable =
        PageRequest.of(Math.max(query.getPageIndex() - 1, 0), query.getPageSize(), sort);

    Page<Post> page = postJpa.findAll(spec, pageable);
    return new PageResponse<>(
        (int) page.getTotalElements(),
        query.getPageSize(),
        query.getPageIndex(),
        toResponseList(page.getContent()));
  }

  /**
   * 构造查询条件：公司维度先落到部门Id集合，其余按部门 / 职级精确、名称模糊
   *
   * @param query 分页查询条件
   * @return 查询条件
   */
  private Specification<Post> buildSpec(GetPagePostsQuery query) {
    Set<Long> companyDepartmentIds =
        query.getCompanyId() == null ? null : findDepartmentIdsByCompany(query.getCompanyId());
    return (root, criteriaQuery, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      if (companyDepartmentIds != null) {
        predicates.add(root.get("departmentId").in(companyDepartmentIds));
      }
      if (query.getDepartmentId() != null) {
        predicates.add(cb.equal(root.get("departmentId"), query.getDepartmentId()));
      }
      if (query.getRankId() != null) {
        predicates.add(cb.equal(root.get("rankId"), query.getRankId()));
      }
      if (StringUtils.isNotBlank(query.getName())) {
        predicates.add(cb.like(root.get("name"), "%" + query.getName().trim() + "%"));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 按公司Id反查其下部门Id集合
   *
   * @param companyId 公司Id
   * @return 部门Id集合
   */
  private Set<Long> findDepartmentIdsByCompany(long companyId) {
    Specification<Department> spec =
        (root, query, cb) -> cb.equal(root.get("companyId"), companyId);
    Set<Long> ids = new HashSet<>();
    for (Department item : departmentJpa.findAll(spec)) {
      ids.add(item.getId());
    }
    return ids;
  }

  /**
   * 批量转响应
   *
   * @param list 岗位列表
   * @return 岗位响应列表
   */
  private List<PostResponse> toResponseList(List<Post> list) {
    if (list.isEmpty()) {
      return List.of();
    }
    LookupCache cache = new LookupCache(list);
    List<PostResponse> result = new ArrayList<>(list.size());
    for (Post item : list) {
      result.add(toResponse(item, cache));
    }
    return result;
  }

  /**
   * 聚合转响应
   *
   * @param model 岗位聚合
   * @param cache 关联实体缓存
   * @return 岗位响应
   */
  private PostResponse toResponse(Post model, LookupCache cache) {
    Department department = cache.departments().get(model.getDepartmentId());
    Company company =
        department == null ? null : cache.companies().get(department.getCompanyId());
    Rank rank = cache.ranks().get(model.getRankId());

    PostResponse response = new PostResponse();
    response.setId(model.getId());
    response.setName(model.getName());
    response.setCompanyId(company == null ? 0L : company.getId());
    response.setCompanyCode(company == null ? "" : company.getCode());
    response.setCompanyName(company == null ? "" : company.getName());
    response.setDepartmentId(department == null ? 0L : department.getId());
    response.setDepartmentCode(department == null ? "" : department.getCode());
    response.setDepartmentName(department == null ? "" : department.getName());
    response.setRankId(rank == null ? 0L : rank.getId());
    response.setRankName(rank == null ? "" : rank.getName());
    response.setRemark(model.getRemark());

    List<RoleResponse> roles = new ArrayList<>(model.getPostRoles().size());
    for (Post_Role postRole : model.getPostRoles()) {
      Role role = cache.roles().get(postRole.getRoleId());
      if (role != null) {
        RoleResponse roleResponse = new RoleResponse();
        roleResponse.setId(role.getId());
        roleResponse.setName(role.getName());
        roleResponse.setRemark(role.getRemark());
        roles.add(roleResponse);
      }
    }
    response.setRoles(roles);
    return response;
  }

  /**
   * 关联实体缓存：按传入的岗位集合一次性把部门 / 公司 / 职级 / 角色查齐
   */
  private final class LookupCache {

    /** 部门缓存（Id → 部门） */
    private final Map<Long, Department> departments = new HashMap<>();

    /** 公司缓存（Id → 公司） */
    private final Map<Long, Company> companies = new HashMap<>();

    /** 职级缓存（Id → 职级） */
    private final Map<Long, Rank> ranks = new HashMap<>();

    /** 角色缓存（Id → 角色） */
    private final Map<Long, Role> roles = new HashMap<>();

    /**
     * 构造并预热四个缓存
     *
     * @param posts 岗位列表
     */
    LookupCache(List<Post> posts) {
      List<Long> departmentIds = posts.stream().map(Post::getDepartmentId).distinct().toList();
      if (!departmentIds.isEmpty()) {
        for (Department item : departmentJpa.findAllById(departmentIds)) {
          departments.put(item.getId(), item);
        }
      }

      List<Long> companyIds =
          departments.values().stream().map(Department::getCompanyId).distinct().toList();
      if (!companyIds.isEmpty()) {
        for (Company item : companyJpa.findAllById(companyIds)) {
          companies.put(item.getId(), item);
        }
      }

      List<Long> rankIds = posts.stream().map(Post::getRankId).distinct().toList();
      if (!rankIds.isEmpty()) {
        for (Rank item : rankJpa.findAllById(rankIds)) {
          ranks.put(item.getId(), item);
        }
      }

      Set<Long> roleIds = new HashSet<>();
      for (Post item : posts) {
        for (Post_Role postRole : item.getPostRoles()) {
          roleIds.add(postRole.getRoleId());
        }
      }
      if (!roleIds.isEmpty()) {
        for (Role item : roleJpa.findAllById(roleIds)) {
          roles.put(item.getId(), item);
        }
      }
    }

    Map<Long, Department> departments() {
      return departments;
    }

    Map<Long, Company> companies() {
      return companies;
    }

    Map<Long, Rank> ranks() {
      return ranks;
    }

    Map<Long, Role> roles() {
      return roles;
    }
  }
}
