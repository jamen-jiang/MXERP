package com.mxerp.system.domain.user;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 用户 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "User",
    indexes = {@Index(name = "IX_User_Name", columnList = "Name")
      // 注意：UserName 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class User extends UpdateAggregateRoot {

  /** 用户名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 账号 */
  @Nationalized
  @Column(name = "UserName", nullable = false, length = 50)
  private String userName;

  /** 邮箱（可空） */
  @Nationalized
  @Column(name = "Email", length = 200)
  @Nullable
  private String email;

  /** 密码 */
  @Nationalized
  @Column(name = "Password", nullable = false, length = 50)
  private String password;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 用户岗位列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "UserId")
  @SQLRestriction("IsDeleted = 0")
  private List<User_Post> userPosts = new ArrayList<>();

  /** 用户角色列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "UserId")
  @SQLRestriction("IsDeleted = 0")
  private List<User_Role> userRoles = new ArrayList<>();

  /**
   * 获取用户岗位列表（只读）
   *
   * @return 不可变视图
   */
  public List<User_Post> getUserPosts() {
    return Collections.unmodifiableList(userPosts);
  }

  /**
   * 获取用户角色列表（只读）
   *
   * @return 不可变视图
   */
  public List<User_Role> getUserRoles() {
    return Collections.unmodifiableList(userRoles);
  }

  /**
   * 构造器
   *
   * @param name 用户名称
   * @param userName 账号
   * @param email 邮箱（可空）
   * @param password 密码
   * @param remark 备注（可空）
   * @param postIds 岗位Id列表
   * @param roleIds 角色Id列表（可空）
   */
  User(
      String name,
      String userName,
      @Nullable String email,
      String password,
      @Nullable String remark,
      List<Long> postIds,
      @Nullable List<Long> roleIds) {
    String trimmedName = StringUtils.trimToNull(name);
    String trimmedUserName = StringUtils.trimToNull(userName);
    if (trimmedName == null) {
      throw new DomainException("用户名称不能为空");
    }
    if (trimmedUserName == null) {
      throw new DomainException("账号不能为空");
    }
    if (StringUtils.isBlank(password)) {
      throw new DomainException("密码不能为空");
    }
    if (postIds.isEmpty()) {
      throw new DomainException("岗位不允许为空!");
    }
    this.name = trimmedName;
    this.userName = trimmedUserName;
    this.email = StringUtils.trimToNull(email);
    this.password = password;
    this.remark = remark;
    syncPosts(postIds);
    syncRoles(roleIds);
  }

  /**
   * 修改用户
   *
   * @param name 用户名称
   * @param userName 账号
   * @param email 邮箱（可空）
   * @param password 密码
   * @param remark 备注（可空）
   * @param postIds 岗位Id列表
   * @param roleIds 角色Id列表（可空）
   */
  void modify(
      String name,
      String userName,
      @Nullable String email,
      String password,
      @Nullable String remark,
      List<Long> postIds,
      @Nullable List<Long> roleIds) {
    String trimmedName = StringUtils.trimToNull(name);
    String trimmedUserName = StringUtils.trimToNull(userName);
    if (trimmedName == null) {
      throw new DomainException("用户名称不能为空");
    }
    if (trimmedUserName == null) {
      throw new DomainException("账号不能为空");
    }
    if (StringUtils.isBlank(password)) {
      throw new DomainException("密码不能为空");
    }
    if (postIds.isEmpty()) {
      throw new DomainException("岗位不允许为空!");
    }
    this.name = trimmedName;
    this.userName = trimmedUserName;
    this.email = StringUtils.trimToNull(email);
    this.password = password;
    this.remark = remark;
    syncPosts(postIds);
    syncRoles(roleIds);
  }

  /**
   * 同步岗位列表
   *
   * @param postIds 岗位Id列表
   */
  private void syncPosts(List<Long> postIds) {

    // 1. 删除未在输入中出现的岗位
    List<User_Post> toRemoves =
        userPosts.stream()
            .filter(a -> postIds.stream().noneMatch(i -> Objects.equals(i, a.getPostId())))
            .toList();
    for (User_Post item : toRemoves) {
      deletePost(item.getId());
    }

    // 2. 添加输入中不存在的岗位
    for (Long postId : postIds) {
      boolean exists =
          userPosts.stream()
              .anyMatch(x -> Objects.equals(x.getPostId(), postId) && !x.getIsDeleted());
      if (!exists) {
        userPosts.add(new User_Post(postId));
      }
    }
  }

  /**
   * 同步角色列表
   *
   * @param roleIds 角色Id列表（可空）
   */
  private void syncRoles(@Nullable List<Long> roleIds) {
    List<Long> target = roleIds == null ? List.of() : roleIds;

    // 1. 删除未在输入中出现的角色
    List<User_Role> toRemoves =
        userRoles.stream()
            .filter(a -> target.stream().noneMatch(i -> Objects.equals(i, a.getRoleId())))
            .toList();
    for (User_Role item : toRemoves) {
      deleteRole(item.getId());
    }

    // 2. 添加输入中不存在的角色
    for (Long roleId : target) {
      boolean exists =
          userRoles.stream()
              .anyMatch(x -> Objects.equals(x.getRoleId(), roleId) && !x.getIsDeleted());
      if (!exists) {
        userRoles.add(new User_Role(roleId));
      }
    }
  }

  /**
   * 删除岗位
   *
   * @param userPostId 用户岗位关联Id
   */
  private void deletePost(Long userPostId) {
    userPosts.stream()
        .filter(x -> Objects.equals(x.getId(), userPostId))
        .findFirst()
        .ifPresent(User_Post::delete);
  }

  /**
   * 删除角色
   *
   * @param userRoleId 用户角色关联Id
   */
  private void deleteRole(Long userRoleId) {
    userRoles.stream()
        .filter(x -> Objects.equals(x.getId(), userRoleId))
        .findFirst()
        .ifPresent(User_Role::delete);
  }
}
