package com.mxerp.system.domain.rank;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import java.util.ArrayList;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级领域服务，负责层级编排 */
@NullMarked
public class RankDomainService {

  /** 职级仓储 */
  private final RankRepository rankRepository;

  /**
   * 构造器注入
   *
   * @param rankRepository 职级仓储
   */
  public RankDomainService(RankRepository rankRepository) {
    this.rankRepository = rankRepository;
  }

  /**
   * 修改职级（含层级编排）
   *
   * @param id 职级Id
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 需要持久化的聚合列表（自身 + 受影响的子孙）
   * @throws DomainException 同级职级名称已存在时抛出
   * @throws DomainException 职级不存在时抛出
   * @throws DomainException 父职级不存在时抛出
   */
  public List<Rank> modify(
      long id,
      @Nullable Long parentId,
      String name,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 1. 查职级
    Rank model = rankRepository.get(id);
    if (model == null) {
      throw new DomainException("职级不存在!");
    }
    // 2. 委托给使用已有聚合的重载
    modify(model, parentId, name, sort, remark);
    // 3. 递归子级
    List<Rank> list = new ArrayList<>();
    setChildrenLevel(model, list);
    list.addFirst(model);
    return list;
  }

  /**
   * 修改职级（使用已有聚合，避免重复查询）
   *
   * @param model 职级聚合
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 同级职级名称已存在时抛出
   */
  public void modify(
      Rank model,
      @Nullable Long parentId,
      String name,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验同级职级名称唯一（排除自身）
    if (rankRepository.isExist(name, parentId, model.getId())) {
      throw new DomainException("同级职级名称" + name + "已存在!");
    }
    model.modify(
        HierarchyUtils.resolveLevel(parentId, getParent(parentId), "父职级"),
        parentId,
        name,
        sort,
        remark);
  }

  /**
   * 删除自身及所有子孙职级
   *
   * @param id 职级Id
   * @return 需要持久化的聚合列表（已标记删除）
   */
  public List<Rank> removeWithChildren(long id) {
    List<Rank> list = rankRepository.getWithChildren(id);
    for (Rank item : list) {
      item.delete();
    }
    return list;
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

  /**
   * 递归设置子职级层级
   *
   * @param model 职级
   * @param list 子职级收集列表
   */
  private void setChildrenLevel(Rank model, List<Rank> list) {
    if (model.getId() == 0) {
      return;
    }
    List<Rank> children = rankRepository.getByParentId(model.getId());
    HierarchyUtils.resetChildrenLevel(model, children);
    for (Rank child : children) {
      setChildrenLevel(child, list);
      list.add(child);
    }
  }
}
