package com.mxerp.shared.infrastructure.security.user;

import lombok.Getter;
import org.jspecify.annotations.NonNull;
import org.jspecify.annotations.Nullable;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.io.Serial;
import java.io.Serializable;
import java.util.Collection;
import java.util.List;
import java.util.Set;

/** 登录用户 */
@Getter
public class LoginUser implements UserDetails, Serializable {

  /** 序列化 ID */
  @Serial private static final long serialVersionUID = 1L;

  /** 用户Id */
  @NonNull private final Long userId;

  /** 账号 */
  @NonNull private final String userName;

  /** 密码 */
  @NonNull private final String password;

  /** 用户名称 */
  @NonNull private final String name;

  /** 手机号（可空） */
  private final String mobilePhone;

  /** 邮箱（可空） */
  private final String email;

  /** 是否超级管理员 */
  private final boolean superAdmin;

  /** 应用Id列表 */
  @NonNull private final Set<Long> appIds;

  /** 许可证列表（即权限列表），构造器中兜底为空集合 */
  @NonNull private final Set<String> licenses;

  /** 公司Id列表，构造器中兜底为空集合 */
  @NonNull private final Set<Long> companyIds;

  /** 部门Id列表，构造器中兜底为空集合 */
  @NonNull private final Set<Long> departmentIds;

  /** 岗位Id列表，构造器中兜底为空集合 */
  @NonNull private final Set<Long> postIds;

  /**
   * 全参构造器
   *
   * @param userId 用户Id
   * @param userName 账号
   * @param password 密码
   * @param name 用户名称
   * @param mobilePhone 手机号（可空）
   * @param email 邮箱（可空）
   * @param superAdmin 是否超级管理员
   * @param appIds 应用Id列表（可空）
   * @param licenses 许可证列表（可空）
   * @param companyIds 公司Id列表（可空）
   * @param departmentIds 部门Id列表（可空）
   * @param postIds 岗位Id列表（可空）
   */
  public LoginUser(
      @NonNull Long userId,
      @NonNull String userName,
      @NonNull String password,
      @NonNull String name,
      @Nullable String mobilePhone,
      @Nullable String email,
      boolean superAdmin,
      @Nullable Set<Long> appIds,
      @Nullable Set<String> licenses,
      @Nullable Set<Long> companyIds,
      @Nullable Set<Long> departmentIds,
      @Nullable Set<Long> postIds) {
    this.userId = userId;
    this.userName = userName;
    this.password = password;
    this.name = name;
    this.mobilePhone = mobilePhone;
    this.email = email;
    this.superAdmin = superAdmin;
    this.appIds = appIds == null ? Set.of() : appIds;
    this.licenses = licenses == null ? Set.of() : licenses;
    this.companyIds = companyIds == null ? Set.of() : companyIds;
    this.departmentIds = departmentIds == null ? Set.of() : departmentIds;
    this.postIds = postIds == null ? Set.of() : postIds;
  }

  /**
   * 权限列表
   *
   * @return 权限列表
   */
  @Override
  public @NonNull Collection<? extends GrantedAuthority> getAuthorities() {
    return List.of();
  }

  /**
   * 账号
   *
   * @return 账号
   */
  @Override
  public @NonNull String getUsername() {
    return userName;
  }

  /**
   * 账号是否未过期
   *
   * @return true
   */
  @Override
  public boolean isAccountNonExpired() {
    return true;
  }

  /**
   * 账号是否未锁定
   *
   * @return true
   */
  @Override
  public boolean isAccountNonLocked() {
    return true;
  }

  /**
   * 凭证是否未过期
   *
   * @return true
   */
  @Override
  public boolean isCredentialsNonExpired() {
    return true;
  }

  /**
   * 账号是否启用
   *
   * @return true
   */
  @Override
  public boolean isEnabled() {
    return true;
  }
}
