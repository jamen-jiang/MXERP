package com.mxerp.system.domain.post;

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

/** 岗位 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Post",
    indexes = {
      @Index(name = "IX_Post_Name", columnList = "Name"),
      @Index(name = "IX_Post_DepartmentId", columnList = "DepartmentId"),
      @Index(name = "IX_Post_RankId", columnList = "RankId")
    })
public class Post extends UpdateAggregateRoot {

  /** 岗位名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 部门Id */
  @Column(name = "DepartmentId", nullable = false)
  private Long departmentId;

  /** 职级Id */
  @Column(name = "RankId", nullable = false)
  private Long rankId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 岗位角色列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "PostId")
  @SQLRestriction("IsDeleted = 0")
  private List<Post_Role> postRoles = new ArrayList<>();

  /**
   * 获取岗位角色列表（只读）
   *
   * @return 不可变视图
   */
  public List<Post_Role> getPostRoles() {
    return Collections.unmodifiableList(postRoles);
  }

  /**
   * 构造器
   *
   * @param name 岗位名称
   * @param departmentId 部门Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   */
  Post(String name, long departmentId, long rankId, @Nullable String remark, List<Long> roleIds) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("岗位名称不能为空");
    }
    this.name = trimmedName;
    this.departmentId = departmentId;
    this.rankId = rankId;
    this.remark = remark;
    syncRoles(roleIds);
  }

  /**
   * 修改岗位
   *
   * @param name 岗位名称
   * @param departmentId 部门Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   */
  void modify(
      String name, long departmentId, long rankId, @Nullable String remark, List<Long> roleIds) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("岗位名称不能为空");
    }
    this.name = trimmedName;
    this.departmentId = departmentId;
    this.rankId = rankId;
    this.remark = remark;
    syncRoles(roleIds);
  }

  /**
   * 删除角色
   *
   * @param roleId 角色Id
   */
  void deleteRole(long roleId) {
    Post_Role item =
        postRoles.stream()
            .filter(x -> Objects.equals(x.getId(), roleId) && !x.getIsDeleted())
            .findFirst()
            .orElse(null);
    if (item != null) {
      item.delete();
    }
  }

  /**
   * 同步角色列表
   *
   * @param roleIds 角色Id列表
   */
  private void syncRoles(List<Long> roleIds) {
    // 1. 删除未在输入中出现的角色
    List<Post_Role> toRemoves =
        postRoles.stream()
            .filter(a -> roleIds.stream().noneMatch(i -> Objects.equals(i, a.getRoleId())))
            .toList();
    for (Post_Role item : toRemoves) {
      deleteRole(item.getId());
    }
    // 2. 添加输入中不存在的角色
    for (Long roleId : roleIds) {
      if (postRoles.stream().noneMatch(x -> Objects.equals(x.getRoleId(), roleId))) {
        postRoles.add(new Post_Role(roleId));
      }
    }
  }
}
