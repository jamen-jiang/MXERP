package com.mxerp.system.domain.post;

import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
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
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   * @return 岗位聚合
   * @throws DomainException 岗位名称已存在时抛出
   */
  public Post create(
      String name, long departmentId, long rankId, @Nullable String remark, List<Long> roleIds) {
    // 校验岗位名称唯一
    if (postRepository.isExistByNameExcludeId(name)) {
      throw new DomainException("岗位" + name + "已存在!");
    }
    return new Post(name, departmentId, rankId, remark, roleIds);
  }
}
