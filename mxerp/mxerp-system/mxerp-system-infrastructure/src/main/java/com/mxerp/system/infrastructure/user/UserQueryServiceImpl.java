package com.mxerp.system.infrastructure.user;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.post.query.PostResponse;
import com.mxerp.system.application.role.query.RoleResponse;
import com.mxerp.system.application.user.query.GetPageUsersQuery;
import com.mxerp.system.application.user.query.UserQueryService;
import com.mxerp.system.application.user.query.UserResponse;
import com.mxerp.system.domain.department.Department;
import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.Post_Role;
import com.mxerp.system.domain.role.Role;
import com.mxerp.system.domain.user.User;
import com.mxerp.system.domain.user.User_Post;
import com.mxerp.system.domain.user.User_Role;
import com.mxerp.system.infrastructure.department.DepartmentJpa;
import com.mxerp.system.infrastructure.role.RoleJpa;
import com.mxerp.system.infrastructure.post.PostJpa;
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
 * 用户查询服务实现（JPA Specification）
 *
 * <p>用户聚合内已带 userRoles / userPosts，角色和岗位名称再各查一次批量填充。
 */
@NullMarked
@Service
public class UserQueryServiceImpl implements UserQueryService {

  /** 用户 JPA 接口 */
  private final UserJpa userJpa;

  /** 角色 JPA 接口 */
  private final RoleJpa roleJpa;

  /** 岗位 JPA 接口 */
  private final PostJpa postJpa;

  /** 部门 JPA 接口 */
  private final DepartmentJpa departmentJpa;

  /**
   * 构造器注入
   *
   * @param userJpa 用户 JPA 接口
   * @param roleJpa 角色 JPA 接口
   * @param postJpa 岗位 JPA 接口
   * @param departmentJpa 部门 JPA 接口
   */
  public UserQueryServiceImpl(
      UserJpa userJpa, RoleJpa roleJpa, PostJpa postJpa, DepartmentJpa departmentJpa) {
    this.userJpa = userJpa;
    this.roleJpa = roleJpa;
    this.postJpa = postJpa;
    this.departmentJpa = departmentJpa;
  }

  /**
   * 按Id查询用户
   *
   * @param id 用户Id
   * @return 用户响应，不存在返回 null
   */
  @Override
  @Nullable
  public UserResponse getById(long id) {
    User model = userJpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 按账号查询用户详情（含角色列表 + 岗位列表）
   *
   * @param userName 账号
   * @return 用户响应，不存在返回 null
   */
  @Override
  @Nullable
  public UserResponse getByUserName(String userName) {
    User model = userJpa.findByUserName(userName).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 分页查询用户
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @Override
  public PageResponse<UserResponse> getPage(GetPageUsersQuery query) {
    Specification<User> spec = buildSpec(query);
    Sort sort = Sort.by(Sort.Direction.ASC, "name");
    PageRequest pageable =
        PageRequest.of(Math.max(query.getPageIndex() - 1, 0), query.getPageSize(), sort);

    Page<User> page = userJpa.findAll(spec, pageable);
    List<UserResponse> list = new ArrayList<>(page.getContent().size());
    for (User item : page.getContent()) {
      list.add(toResponse(item));
    }
    return new PageResponse<>(
        (int) page.getTotalElements(), query.getPageSize(), query.getPageIndex(), list);
  }

  /**
   * 构造查询条件：组织维度先落到用户Id集合，名称 / 账号 / 邮箱走模糊匹配
   *
   * @param query 分页查询条件
   * @return 查询条件
   */
  private Specification<User> buildSpec(GetPageUsersQuery query) {
    Set<Long> filterUserIds = resolveFilterUserIds(query);
    return (root, criteriaQuery, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      if (filterUserIds != null) {
        predicates.add(root.get("id").in(filterUserIds));
      }
      if (StringUtils.isNotBlank(query.getName())) {
        predicates.add(cb.like(root.get("name"), "%" + query.getName().trim() + "%"));
      }
      if (StringUtils.isNotBlank(query.getUserName())) {
        predicates.add(cb.like(root.get("userName"), "%" + query.getUserName().trim() + "%"));
      }
      if (StringUtils.isNotBlank(query.getEmail())) {
        predicates.add(cb.like(root.get("email"), "%" + query.getEmail().trim() + "%"));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 按公司 / 部门 / 岗位 / 职级维度反查用户Id集合，任一条件都为空则返回 null（不过滤）
   *
   * @param query 分页查询条件
   * @return 用户Id集合，或 null
   */
  @Nullable
  private Set<Long> resolveFilterUserIds(GetPageUsersQuery query) {
    Set<Long> postIds = resolveFilterPostIds(query);
    if (postIds == null) {
      return null;
    }
    if (postIds.isEmpty()) {
      return Set.of();
    }
    return new HashSet<>(userJpa.findListByPostIds(postIds).stream().map(User::getId).toList());
  }

  /**
   * 按组织维度反查岗位Id集合，任一条件都为空则返回 null（不过滤）
   *
   * @param query 分页查询条件
   * @return 岗位Id集合，或 null
   */
  @Nullable
  private Set<Long> resolveFilterPostIds(GetPageUsersQuery query) {
    if (query.getPostId() != null) {
      return Set.of(query.getPostId());
    }

    if (query.getDepartmentId() != null) {
      Specification<Post> spec =
          (root, criteriaQuery, cb) ->
              cb.equal(root.get("departmentId"), query.getDepartmentId());
      return collectIds(postJpa.findAll(spec));
    }

    if (query.getRankId() != null) {
      Specification<Post> spec =
          (root, criteriaQuery, cb) -> cb.equal(root.get("rankId"), query.getRankId());
      return collectIds(postJpa.findAll(spec));
    }

    if (query.getCompanyId() != null) {
      Specification<Department> spec =
          (root, criteriaQuery, cb) -> cb.equal(root.get("companyId"), query.getCompanyId());
      List<Long> departmentIds =
          departmentJpa.findAll(spec).stream().map(Department::getId).toList();
      if (departmentIds.isEmpty()) {
        return Set.of();
      }
      Specification<Post> postSpec =
          (root, criteriaQuery, cb) -> root.get("departmentId").in(departmentIds);
      return collectIds(postJpa.findAll(postSpec));
    }

    return null;
  }

  /**
   * 提取岗位列表上的 Id
   *
   * @param list 岗位列表
   * @return 岗位Id集合
   */
  private Set<Long> collectIds(List<Post> list) {
    Set<Long> ids = new HashSet<>();
    for (Post item : list) {
      ids.add(item.getId());
    }
    return ids;
  }

  /**
   * 聚合转响应
   *
   * @param model 用户聚合
   * @return 用户响应
   */
  private UserResponse toResponse(User model) {
    // 角色
    Map<Long, Role> roleMap = new HashMap<>();
    Set<Long> roleIds = new HashSet<>();
    for (User_Role userRole : model.getUserRoles()) {
      roleIds.add(userRole.getRoleId());
    }
    if (!roleIds.isEmpty()) {
      for (Role item : roleJpa.findAllById(roleIds)) {
        roleMap.put(item.getId(), item);
      }
    }

    // 岗位
    Map<Long, Post> postMap = new HashMap<>();
    Set<Long> postIds = new HashSet<>();
    for (User_Post userPost : model.getUserPosts()) {
      postIds.add(userPost.getPostId());
    }
    if (!postIds.isEmpty()) {
      for (Post item : postJpa.findAllById(postIds)) {
        postMap.put(item.getId(), item);
      }
    }

    UserResponse response = new UserResponse();
    response.setId(model.getId());
    response.setName(model.getName());
    response.setUserName(model.getUserName());
    response.setEmail(model.getEmail());
    response.setPassword(model.getPassword());
    response.setRemark(model.getRemark());

    List<RoleResponse> roles = new ArrayList<>();
    for (User_Role userRole : model.getUserRoles()) {
      Role role = roleMap.get(userRole.getRoleId());
      if (role != null) {
        RoleResponse roleResponse = new RoleResponse();
        roleResponse.setId(role.getId());
        roleResponse.setName(role.getName());
        roleResponse.setRemark(role.getRemark());
        roles.add(roleResponse);
      }
    }
    response.setRoles(roles);

    // 岗位只填基本字段，公司/部门/职级名称留给 Post 查询接口
    List<PostResponse> posts = new ArrayList<>();
    for (User_Post userPost : model.getUserPosts()) {
      Post post = postMap.get(userPost.getPostId());
      if (post == null) {
        continue;
      }
      PostResponse postResponse = new PostResponse();
      postResponse.setId(post.getId());
      postResponse.setName(post.getName());
      postResponse.setDepartmentId(post.getDepartmentId() == null ? 0L : post.getDepartmentId());
      postResponse.setRankId(post.getRankId() == null ? 0L : post.getRankId());
      postResponse.setRemark(post.getRemark());
      posts.add(postResponse);
    }
    response.setPosts(posts);
    return response;
  }
}
