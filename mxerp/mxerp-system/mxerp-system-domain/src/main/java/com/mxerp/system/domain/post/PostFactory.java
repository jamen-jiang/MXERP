package com.mxerp.system.domain.post;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 岗位工厂，负责创建岗位 */
@NullMarked
public class PostFactory {

  /** 岗位仓储 */
  private final PostRepository postRepository;

  /**
   * 构造器注入
   *
   * @param postRepository 岗位仓储
   */
  public PostFactory(PostRepository postRepository) {
    this.postRepository = postRepository;
  }

  /**
   * 创建岗位
   *
   * @param name 岗位名称
   * @param departmentId 部门Id
   * @param parentId 上级岗位Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @return 岗位聚合
   * @throws DomainException 岗位名称已存在、或上级岗位不存在时抛出
   */
  public Post create(
      String name,
      long departmentId,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable String remark) {
    // 校验岗位名称唯一
    if (postRepository.isExistByNameExcludeId(name)) {
      throw new DomainException("岗位" + name + "已存在!");
    }
    return new Post(
        name,
        departmentId,
        parentId,
        HierarchyUtils.resolveParentPath(parentId, getParent(parentId), "上级岗位"),
        disabled,
        remark);
  }

  /**
   * 查询上级岗位
   *
   * @param parentId 上级岗位Id（可空）
   * @return 上级岗位，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private Post getParent(@Nullable Long parentId) {
    return parentId == null ? null : postRepository.get(parentId);
  }
}