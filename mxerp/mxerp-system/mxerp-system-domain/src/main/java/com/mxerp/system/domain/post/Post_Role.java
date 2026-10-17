package com.mxerp.system.domain.post;

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

/** 岗位绑定的角色 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Post_Role",
    indexes = {
      @Index(name = "IX_Post_Role_PostId", columnList = "PostId"),
      @Index(name = "IX_Post_Role_RoleId", columnList = "RoleId")
    })
public class Post_Role extends BaseDeleteEntity {

  /** 岗位Id */
  @Column(name = "PostId", nullable = false)
  private Long postId;

  /** 角色Id */
  @Column(name = "RoleId", nullable = false)
  private Long roleId;

  /**
   * 构造器
   *
   * @param roleId 角色Id
   */
  public Post_Role(Long roleId) {
    this.roleId = roleId;
  }
}
