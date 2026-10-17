package com.mxerp.system.domain.rank;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级工厂，负责创建职级 */
@NullMarked
public class RankFactory {

  /** 职级仓储 */
  private final RankRepository rankRepository;

  /**
   * 构造器注入
   *
   * @param rankRepository 职级仓储
   */
  public RankFactory(RankRepository rankRepository) {
    this.rankRepository = rankRepository;
  }

  /**
   * 创建职级
   *
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 职级聚合
   * @throws DomainException 同级职级名称已存在时抛出
   * @throws DomainException 父职级不存在时抛出
   */
  public Rank create(
      @Nullable Long parentId, String name, @Nullable Integer sort, @Nullable String remark) {
    // 校验同级职级名称唯一
    if (rankRepository.isExist(name, parentId)) {
      throw new DomainException("同级职级名称" + name + "已存在!");
    }
    return new Rank(
        HierarchyUtils.resolveLevel(parentId, getParent(parentId), "父职级"),
        parentId,
        name,
        sort,
        remark);
  }

  /**
   * 查询父职级
   *
   * @param parentId 父Id（可空）
   * @return 父职级，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private Rank getParent(@Nullable Long parentId) {
    return parentId == null ? null : rankRepository.get(parentId);
  }
}
