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

/** 用户绑定的岗位 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "User_Post",
    indexes = {
      @Index(name = "IX_User_Post_UserId", columnList = "UserId"),
      @Index(name = "IX_User_Post_PostId", columnList = "PostId")
    })
public class User_Post extends BaseDeleteEntity {

  /** 用户Id */
  @Column(name = "UserId", nullable = false)
  private Long userId;

  /** 岗位Id */
  @Column(name = "PostId", nullable = false)
  private Long postId;

  /**
   * 构造器
   *
   * @param postId 岗位Id
   */
  public User_Post(Long postId) {
    this.postId = postId;
  }
}
