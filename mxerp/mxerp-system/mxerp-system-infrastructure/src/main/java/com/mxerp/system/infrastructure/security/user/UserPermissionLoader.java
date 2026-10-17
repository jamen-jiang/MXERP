package com.mxerp.system.infrastructure.security.user;

import com.mxerp.system.domain.app.App;
import com.mxerp.system.domain.app.AppTypeEnum;
import com.mxerp.system.domain.department.Department;
import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.Post_Role;
import com.mxerp.system.domain.privilege.Privilege;
import com.mxerp.system.domain.rank.Rank;
import com.mxerp.system.domain.user.User;
import com.mxerp.system.domain.user.User_Post;
import com.mxerp.system.domain.user.User_Role;
import com.mxerp.system.infrastructure.app.AppJpa;
import com.mxerp.system.infrastructure.department.DepartmentJpa;
import com.mxerp.system.infrastructure.post.PostJpa;
import com.mxerp.system.infrastructure.privilege.PrivilegeJpa;
import com.mxerp.system.infrastructure.rank.RankJpa;
import com.mxerp.system.infrastructure.user.UserJpa;
import com.mxerp.shared.infrastructure.security.user.LoginUser;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.function.Function;
import java.util.stream.Collectors;
import org.apache.commons.lang3.StringUtils;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

/**
 * 用户权限加载器：登录时一次性算全「数据范围 + 应用许可」，避免每个请求都递归查组织树
 *
 * <p>数据范围口径：用户 → 岗位 → 部门/职级，再沿部门树、职级树**向下展开到子孙**，所以上级能看到
 * 自己及所有下级的岗位。
 */
@Service
public class UserPermissionLoader {

  /** 超级管理员角色Id */
  private static final Long ADMIN_ROLE_ID = 1L;

  /** 权限主体：用户 */
  private static final String MASTER_USER = "User";

  /** 权限主体：角色 */
  private static final String MASTER_ROLE = "Role";

  /** 岗位 JPA */
  private final PostJpa postJpa;

  /** 部门 JPA */
  private final DepartmentJpa departmentJpa;

  /** 职级 JPA */
  private final RankJpa rankJpa;

  /** 权限 JPA */
  private final PrivilegeJpa privilegeJpa;

  /** 应用 JPA */
  private final AppJpa appJpa;

  /**
   * 构造器注入
   *
   * @param postJpa 岗位 JPA
   * @param departmentJpa 部门 JPA
   * @param rankJpa 职级 JPA
   * @param privilegeJpa 权限 JPA
   * @param appJpa 应用 JPA
   */
  public UserPermissionLoader(
      PostJpa postJpa,
      DepartmentJpa departmentJpa,
      RankJpa rankJpa,
      PrivilegeJpa privilegeJpa,
      AppJpa appJpa) {
    this.postJpa = postJpa;
    this.departmentJpa = departmentJpa;
    this.rankJpa = rankJpa;
    this.privilegeJpa = privilegeJpa;
    this.appJpa = appJpa;
  }

  /**
   * 加载用户权限数据范围
   *
   * @param model 用户聚合
   * @return 权限数据
   */
  @Transactional(readOnly = true)
  public UserPermissionData load(User model) {
    UserPermissionData data = new UserPermissionData();

    // 1. 用户直接角色、用户岗位（都在聚合内，无需查库）
    Set<Long> userRoleIds =
        model.getUserRoles().stream().map(User_Role::getRoleId).collect(Collectors.toSet());
    Set<Long> userPostIds =
        model.getUserPosts().stream().map(User_Post::getPostId).collect(Collectors.toSet());

    List<Post> userPosts = userPostIds.isEmpty() ? List.of() : postJpa.findAllById(userPostIds);

    // 2. 岗位 → 部门 / 职级（聚合内是 Id，需查实体）
    Set<Long> userDepartmentIds = collectIds(userPosts, Post::getDepartmentId);
    Set<Long> userRankIds = collectIds(userPosts, Post::getRankId);

    // 3. 部门 → 公司（组织的顶层链路终点）
    List<Department> userDepartments =
        userDepartmentIds.isEmpty() ? List.of() : departmentJpa.findAllById(userDepartmentIds);
    data.companyIds = collectIds(userDepartments, Department::getCompanyId);

    // 4. 沿部门树 / 职级树向下展开到子孙，这是「数据范围」的真正口径
    Set<Long> allDepartmentIds =
        userDepartmentIds.isEmpty()
            ? Set.of()
            : collectIds(departmentJpa.findByIdsOrDescendants(userDepartmentIds), Department::getId);
    Set<Long> allRankIds =
        userRankIds.isEmpty()
            ? Set.of()
            : collectIds(rankJpa.findByIdsOrDescendants(userRankIds), Rank::getId);

    // 5. 部门 × 职级交叉出的岗位，才是用户实际可见的岗位
    Set<Long> allPostIds;
    if (allDepartmentIds.isEmpty() || allRankIds.isEmpty()) {
      allPostIds = Set.of();
    } else {
      allPostIds =
          collectIds(postJpa.findByDepartmentIdInAndRankIdIn(allDepartmentIds, allRankIds), Post::getId);
    }
    data.departmentIds = allDepartmentIds;
    data.rankIds = allRankIds;
    data.postIds = allPostIds;

    // 6. 超级管理员走独立分支：直接放开全部应用，不再查权限表
    if (userRoleIds.contains(ADMIN_ROLE_ID)) {
      data.superAdmin = true;
      Set<App> apps = new HashSet<>(appJpa.findAll());
      fillApps(data, apps);
      return data;
    }

    data.superAdmin = false;

    // 7. 岗位聚合内挂的角色 → 角色Id
    Set<Long> postRoleIds = new HashSet<>();
    for (Post item : postJpa.findAllById(allPostIds)) {
      for (Post_Role postRole : item.getPostRoles()) {
        postRoleIds.add(postRole.getRoleId());
      }
    }

    // 8. 权限来源三条：用户直接、用户角色、岗位角色
    Set<Long> appIds = new HashSet<>();
    addAccessValues(
        appIds, privilegeJpa.findByMasterAndMasterValueIn(MASTER_USER, Set.of(model.getId())));
    if (!userRoleIds.isEmpty()) {
      addAccessValues(
          appIds, privilegeJpa.findByMasterAndMasterValueIn(MASTER_ROLE, userRoleIds));
    }
    if (!postRoleIds.isEmpty()) {
      addAccessValues(
          appIds, privilegeJpa.findByMasterAndMasterValueIn(MASTER_ROLE, postRoleIds));
    }

    if (appIds.isEmpty()) {
      return data;
    }

    // 9. 按权限指向的应用反查许可
    fillApps(data, new HashSet<>(appJpa.findByIdIn(appIds)));
    return data;
  }

  /**
   * 提取实体集合上的某个 Id 字段
   *
   * @param list 实体列表
   * @param mapper 取值函数
   * @param <T> 实体类型
   * @return Id 集合
   */
  private <T> Set<Long> collectIds(Collection<T> list, Function<T, Long> mapper) {
    if (list.isEmpty()) {
      return Set.of();
    }
    return list.stream().map(mapper).collect(Collectors.toSet());
  }

  /**
   * 累计权限记录指向的应用Id
   *
   * @param appIds 累计目标集合
   * @param privileges 权限记录集合
   */
  private void addAccessValues(Set<Long> appIds, Set<Privilege> privileges) {
    for (Privilege item : privileges) {
      appIds.add(item.getAccessValue());
    }
  }

  /**
   * 填充应用Id与许可证（过滤掉 RESOURCE 类型的资源节点，它不参与许可）
   *
   * @param data 权限数据
   * @param apps 应用集合
   */
  private void fillApps(UserPermissionData data, Set<App> apps) {
    data.appIds =
        apps.stream()
            .filter(x -> !AppTypeEnum.RESOURCE.getCode().equals(x.getType()))
            .map(App::getId)
            .collect(Collectors.toSet());
    data.licenses =
        apps.stream()
            .map(App::getLicense)
            .filter(StringUtils::isNotBlank)
            .collect(Collectors.toSet());
  }

  /** 用户权限数据 */
  public static class UserPermissionData {
    /** 是否超级管理员 */
    public boolean superAdmin;

    /** 公司Id集合 */
    public Set<Long> companyIds = Set.of();

    /** 部门Id集合（含子孙） */
    public Set<Long> departmentIds = Set.of();

    /** 岗位Id集合 */
    public Set<Long> postIds = Set.of();

    /** 职级Id集合（含子孙） */
    public Set<Long> rankIds = Set.of();

    /** 应用Id集合 */
    public Set<Long> appIds = Set.of();

    /** 许可证集合 */
    public Set<String> licenses = Set.of();
  }
}
