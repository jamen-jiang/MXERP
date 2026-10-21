package com.mxerp.system.domain.user;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 用户绑定的角色 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "User_Role",
    indexes = {
      @Index(name = "IX_User_Role_UserId", columnList = "UserId"),
      @Index(name = "IX_User_Role_RoleId", columnList = "RoleId")
    })
public class User_Role extends BaseDeleteEntity {

  /** 用户Id */
  @Column(name = "UserId", nullable = false)
  private Long userId;

  /** 角色Id */
  @Column(name = "RoleId", nullable = false)
  private Long roleId;

  /**
   * 构造器
   *
   * @param roleId 角色Id
   */
  public User_Role(long roleId) {
    this.roleId = roleId;
  }
}
