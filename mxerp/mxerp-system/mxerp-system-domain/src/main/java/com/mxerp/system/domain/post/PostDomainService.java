package com.mxerp.system.domain.post;

import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 岗位领域服务 */
@NullMarked
public class PostDomainService {

  /** 岗位仓储 */
  private final PostRepository postRepository;

  /**
   * 构造器注入
   *
   * @param postRepository 岗位仓储
   */
  public PostDomainService(PostRepository postRepository) {

    this.postRepository = postRepository;
  }

  /**
   * 修改岗位（按Id查询后修改）
   *
   * @param id 岗位Id
   * @param name 岗位名称
   * @param departmentId 部门Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   * @return 岗位聚合
   * @throws DomainException 岗位不存在时抛出
   * @throws DomainException 岗位名称已存在时抛出
   */
  public Post modify(
      long id,
      String name,
      long departmentId,
      long rankId,
      @Nullable String remark,
      List<Long> roleIds) {
    // 1. 查岗位
    Post model = postRepository.get(id);
    if (model == null) {
      throw new DomainException("岗位不存在!");
    }
    // 2. 委托给使用已有聚合的重载
    modify(model, name, departmentId, rankId, remark, roleIds);
    return model;
  }

  /**
   * 修改岗位（使用已有聚合，避免重复查询）
   *
   * @param post 岗位聚合
   * @param name 岗位名称
   * @param departmentId 部门Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   * @throws DomainException 岗位名称已存在时抛出
   */
  public void modify(
      Post post,
      String name,
      long departmentId,
      long rankId,
      @Nullable String remark,
      List<Long> roleIds) {
    // 校验岗位名称唯一（排除自身）
    if (postRepository.isExistByNameExcludeId(name, post.getId())) {
      throw new DomainException("岗位" + name + "已存在!");
    }
    post.modify(name, departmentId, rankId, remark, roleIds);
  }
}
